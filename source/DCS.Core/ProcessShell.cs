using System;
using System.Diagnostics;

namespace DCS.Core
{
    public class ProcessShell
    {
        private readonly Action<string> _standardOutputCallback;
        private readonly Action<string> _errorOutputCallback;

        public ProcessShell(Action<string> standardOutputCallback = null, Action<string> errorOutputCallback = null)
        {
            _standardOutputCallback = standardOutputCallback ?? Console.WriteLine;
            _errorOutputCallback = errorOutputCallback ?? Console.WriteLine;
        }

        /// <summary>
        /// Executes background process with redirected output.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public int RunBackground(
            string fileName,
            string arguments = null,
            string workingDirectory = null)
        {
            var processStartInfo = new ProcessStartInfo(fileName, arguments);
            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }

            using (var run = ProcessRun.Start(processStartInfo,
                s => _standardOutputCallback(s),
                s => _errorOutputCallback(s)))
            {
                run.Task.Wait();
                return run.Task.Result.ExitCode;
            }
        }

        /// <summary>
        /// Run a background process with redirected output.
        /// </summary>
        /// <param name="startInfo">Start info for process (fileName, args, etc)</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int RunBackground(
            ProcessStartInfo startInfo,
            TimeSpan timeout)
        {
            using (var run = ProcessRun.Start(startInfo))
            {
                run.Task.Wait(timeout);
                run.Kill();
                return run.Process.ExitCode;
            }
        }
    }
}