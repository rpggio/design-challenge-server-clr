using System;
using System.Diagnostics;

namespace GateScheduler.FeatureTests
{
    public static class Shell
    {
        /// <summary>
        /// Executes background process with redirected output.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public static int ExecuteBackround(
          string fileName,
          string arguments = null,
          string workingDirectory = null)
        {
            var processStartInfo = new ProcessStartInfo(fileName, arguments);
            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }
            using (var process = StartBackgroundProcess(processStartInfo))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        /// <summary>
        /// Start a background process with redirected output.
        /// </summary>
        /// <param name="startInfo">Start info for process (fileName, args, etc)</param>
        /// <param name="standardOutputCallback"></param>
        /// <param name="errorOutputCallback"></param>
        /// <returns></returns>
        public static Process StartBackgroundProcess(
            ProcessStartInfo startInfo,
            Action<string> standardOutputCallback = null,
            Action<string> errorOutputCallback = null)
        {
            if (standardOutputCallback == null)
            {
                standardOutputCallback = Console.WriteLine;
            }
            if (errorOutputCallback == null)
            {
                errorOutputCallback = Console.Error.WriteLine;
            }

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += (_, e) => standardOutputCallback(e.Data);
            process.ErrorDataReceived += (_, e) => errorOutputCallback(e.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }

    }
}
