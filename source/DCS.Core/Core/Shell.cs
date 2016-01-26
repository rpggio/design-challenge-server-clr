using System;
using System.Diagnostics;
using System.IO;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public class Shell
    {
        private Action<string> _log;
        private readonly Action<string> _standardOutputCallback;
        private readonly Action<string> _errorOutputCallback;

        public Shell( Action<string> log, 
            Action<string> standardOutputCallback = null, 
            Action<string> errorOutputCallback = null)
        {
            _log = log;
            _standardOutputCallback = standardOutputCallback ?? delegate { };
            _errorOutputCallback = errorOutputCallback ?? delegate { };
        }

        /// <summary>
        ///     Executes background process with redirected output.
        /// </summary>
        public int RunBackground(
            string fileName,
            string arguments = null,
            string workingDirectory = null,
            TextWriter output = null)
        {
            var processStartInfo = new ProcessStartInfo(fileName, arguments);
            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }
            Action<string> writeLine = null;
            if (output != null)
            {
                var syncWriter = TextWriter.Synchronized(output);
                writeLine = syncWriter.WriteLine;
            }
            using (var process = StartBackgroundProcess(processStartInfo, 
                writeLine, writeLine))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        /// <summary>
        ///     Start a background process with redirected output.
        /// </summary>
        /// <param name="startInfo">Start info for process (fileName, args, etc)</param>
        /// <param name="standardOutputCallback"></param>
        /// <param name="errorOutputCallback"></param>
        /// <returns></returns>
        public Process StartBackgroundProcess(
            ProcessStartInfo startInfo,
            Action<string> standardOutputCallback = null,
            Action<string> errorOutputCallback = null)
        {
            if (standardOutputCallback == null)
            {
                standardOutputCallback = _standardOutputCallback;
            }
            if (errorOutputCallback == null)
            {
                errorOutputCallback = _errorOutputCallback;
            }

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var process = new Process {StartInfo = startInfo};
            process.OutputDataReceived += (_, e) => standardOutputCallback(e.Data);
            process.ErrorDataReceived += (_, e) => errorOutputCallback(e.Data);

            var startMessage = string.Format("> {0} {1}", startInfo.FileName,
                string.Join(" ", startInfo.Arguments));
            standardOutputCallback(startMessage);
            _log(startMessage);

            if (!process.Start())
            {
                throw new Exception(string.Format("Failed to start new process {0}. Existing process was returned.",
                    startInfo.FileName));
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }
    }
}