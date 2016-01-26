using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.Core.Net;
using DCS.ServerRuntime.Framework;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent]
    public class SolutionRunner
    {
        private readonly ILog _log;
        private readonly AppSettings _settings;
        private readonly PortReservations _reservations;
        private readonly Shell _shell;

        public SolutionRunner(ILog log, AppSettings settings, PortReservations reservations, Shell shell)
        {
            _log = log;
            _settings = settings;
            _reservations = reservations;
            _shell = shell;
        }

        public AssessmentResult Run(IFile solutionExecutable, IDirectory testDirectory)
        {
            string key = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            var userPassword = new SecureString();
            _settings.Env.SolutionExecPassword.ForEach(userPassword.AppendChar);

            PortAssignment port;
            if (!_reservations.TryReserve(out port))
            {
                return new AssessmentResult(AssessmentOutcome.SystemBusy)
                {
#warning laaaaaame
                    BuildOutput = "Sorry, the system is not able to test the solution at this time. Please push another revision later to try again."
                };
            }
            using (port)
            {
                AssessmentResult result;
                // start process
                Process solutionProcess;
                var solutionOutput = new StringWriter();
                try
                {
                    var solutionArgs = "{0} {1}".FormatFrom(port.Number, key);
                    _log.DebugFormat("{0} {1}", solutionExecutable.Path, solutionArgs);
                    var solutionOutputSync = TextWriter.Synchronized(solutionOutput);
                    solutionProcess = _shell.StartBackgroundProcess(
                        new ProcessStartInfo(solutionExecutable.Path, solutionArgs)
                        {
                            WorkingDirectory = solutionExecutable.Parent().Path
                        },
                        solutionOutputSync.WriteLine,
                        solutionOutputSync.WriteLine);

                    if (solutionProcess == null)
                    {
                        throw new Exception("Failed to start process");
                    }
                }
                catch (Exception ex)
                {
                    _log.Warn("Error starting process {0}".FormatFrom(solutionExecutable), ex);
                    return new AssessmentResult(AssessmentOutcome.SolutionFailure)
                    {
                        BuildOutput = "Error starting solution: {0}".FormatFrom(ex.Summary())
                    };
                }

                try
                {
                    // verify http
                    if (CheckHttpResponse(solutionProcess, port.Number, key, out result))
                    {
                        // run tests
                        result = RunTests(testDirectory, port.Number);
                    }
                }
                finally
                {
                    try
                    {
                        solutionProcess.Kill();
                        solutionProcess.Dispose();
                    }
                    catch
                    {
                    }
                }

                if (result.Outcome == AssessmentOutcome.SolutionFailure)
                {
                    result.BuildOutput = solutionOutput.ToString();
                }

                return result;
            }
        }

        private bool CheckHttpResponse(Process userProcess, int port, string key, out AssessmentResult result)
        {
            string statusContent;
            var webClient = new WebClientExt
            {
                Timeout = TimeSpan.FromSeconds(5),
                Headers = {{"Accept", "application/json"}}
            };
            try
            {
                statusContent =
                    webClient.DownloadString(string.Format("http://localhost:{0}/status", port));
            }
            catch (Exception ex)
            {
                _log.Debug("Process exited early", ex);
                if (userProcess.HasExited)
                {
                    result = new AssessmentResult(AssessmentOutcome.SolutionFailure)
                    {
                        Message = "The solution process terminated early."
                    };
                    return false;
                }

                _log.Debug("Solution HTTP error", ex);
                result = new AssessmentResult(AssessmentOutcome.SolutionFailure)
                {
                    Message = "The solution process [{0}] did not respond to HTTP request '/status' at port {1}."
                        .FormatFrom(Path.GetFileName(userProcess.StartInfo.FileName), port)
                };
                return false;
            }

            bool responseIsValid = !statusContent.IsEmpty();

            if (responseIsValid)
            {
                try
                {
                    var statusObject = JsonConvert.DeserializeObject<JObject>(statusContent);
                    var startupKey = statusObject.Property("startupKey").Value;
                    if (!key.EqualsIgnoreCase(startupKey.ToString()))
                    {
                        responseIsValid = false;
                    }
                }
                catch (Exception ex)
                {
                    _log.Debug("Failed to parse solution HTTP response", ex);
                    responseIsValid = false;
                }
            }
            ;

            if (!responseIsValid)
            {
                _log.Debug("Invalid response from solution HTTP");
                result = new AssessmentResult(AssessmentOutcome.SolutionFailure)
                {
                    Message =
                        "The solution process [{0}] did not respond to HTTP request '/status' with a proper response. \r\n  Expected: {1}\r\n  Received: {2}"
                            .FormatFrom(Path.GetFileName(userProcess.StartInfo.FileName),
                                JsonConvert.SerializeObject(new {startupKey = key}),
                                statusContent)
                };
                return false;
            }

            result = null;
            return true;
        }

        private AssessmentResult RunTests(IDirectory testDirectory, int port)
        {
            const string testOutputFileName = "features_report.html";
            var ruby = new PathFile(_settings.Env.RubyBin, "ruby.exe");
            var cuke = new PathFile(_settings.Env.RubyBin, "cucumber");
            string args = "{0} {1} --strict --format pretty --format html --out={2} {3}={4}"
                .FormatFrom(
                cuke, 
                testDirectory.Path, 
                testOutputFileName,
                "features_test_port", 
                port);

            var stringWriter = new StringWriter();
            var syncWriter = TextWriter.Synchronized(stringWriter);
            using (var process = _shell.StartBackgroundProcess(
                new ProcessStartInfo(ruby.Path, args)
                {
                    UseShellExecute = false,
                    WorkingDirectory = testDirectory.Path,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                syncWriter.WriteLine,
                syncWriter.WriteLine))
            {
                AssessmentResult assessmentResult;
                if (!process.WaitForExit(30000))
                {
                    assessmentResult = new AssessmentResult(AssessmentOutcome.TestFailure)
                    {
                        Message = "Timeout waiting for test completion"
                    };
                }
                else
                {
                    if (process.ExitCode == 0)
                    {
                        assessmentResult = new AssessmentResult(AssessmentOutcome.Success)
                        {
                            Message = "Tests passed"
                        };
                    }
                    else
                    {
                        assessmentResult = new AssessmentResult(AssessmentOutcome.TestFailure)
                        {
                            Message = "Tests failed"
                        };
                    }
                }

                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch(Exception ex)
                {
                    _log.DebugFormat("Failed to kill test process {0} {1}: {2}",
                        ruby.Path,
                        args,
                        ex.Summary());
                }

                var testOutputFile = testDirectory.File(testOutputFileName);
                if (testOutputFile.Exists())
                {
                    assessmentResult.TestOutput = testOutputFile.ReadAllText();
                    assessmentResult.TestOutputFormat = TestOutputFormat.Html;
                }
                assessmentResult.BuildOutput = stringWriter.ToString();
                return assessmentResult;
            }
        }
    }
}