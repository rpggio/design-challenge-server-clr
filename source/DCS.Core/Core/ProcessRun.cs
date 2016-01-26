using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    /// <summary>
    /// Represents a running process, with associated Task object. When disposed, the process will be killed and the process and task disposed.
    /// </summary>
    public class ProcessRun : IDisposable
    {
        public static ProcessRun Start(
            ProcessStartInfo startInfo,
            Action<string> onOutput = null,
            Action<string> onErrorOutput = null)
        {
            if (startInfo.FileName.IsEmpty())
            {
                throw new ArgumentException("startInfo.FileName must be assigned");
            }

            var processTask = new ProcessRun();
            var task = System.Threading.Tasks.Task.Run(() =>
            {
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.LoadUserProfile = false;

                var outputStringWriter = new StringWriter();
                var output = TextWriter.Synchronized(outputStringWriter);
                var errorStringWriter = new StringWriter();
                var error = TextWriter.Synchronized(errorStringWriter);

                var process = new Process {StartInfo = startInfo};
                processTask.Process = process;
                DataReceivedEventHandler handleOutput = (_, e) =>
                {
                    if (onOutput != null)
                    {
                        onOutput(e.Data);
                    }
                    output.WriteLineAsync(e.Data);
                };
                DataReceivedEventHandler handleErrorOutput = (_, e) =>
                {
                    if (onErrorOutput != null)
                    {
                        onErrorOutput(e.Data.Left(1000));
                    }
                    error.WriteLineAsync(e.Data);
                };

                // todo: refactor so these subscriptions can be cleaned up

                process.OutputDataReceived += handleOutput;
                process.ErrorDataReceived += handleErrorOutput;

                try
                {
                    if (!process.Start())
                    {
                        throw new Exception(
                            string.Format("Failed to start new process {0}. Existing process was returned.",
                                startInfo.FileName));
                    }

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
                finally
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }

                // Supposed to call this method twice for async listeners.
                // http://msdn.microsoft.com/en-us/library/ty0d8k56(v=vs.110)
                process.WaitForExit();

                return new ProcessResult(process.ExitCode)
                {
                    StandardOutput = outputStringWriter.ToString(),
                    StandardError = errorStringWriter.ToString()
                };
            });
            processTask.Task = task;

            // this is crap
            Thread.Sleep(100);

            return processTask;
        }


        private bool _disposed;

        /// <summary>
        /// The running process
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Task that can be used to wait for process completion. Process output is captured in the task results.
        /// </summary>
        public Task<ProcessResult> Task { get; private set; }

        public void Kill()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (Process != null)
            {
                try
                {
                    if (!Process.HasExited) // problem here
                    {
                        Process.Kill();
                    }
                }
                catch
                {
                }

                if (Task != null)
                {
                    if (!Task.IsCompleted)
                    {
                        // try to let output capture complete
                        Task.Wait(100);
                    }
                    if (Task.IsCompleted)
                    {
                        Task.Dispose();
                    }
                }
                Process.Dispose();
            }
            else
            {
                if (Task != null)
                {
                    Task.Dispose();
                }
            }
        }
    }
}