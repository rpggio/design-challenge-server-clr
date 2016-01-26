using System;
using System.Diagnostics;
using DCS.Core.IO;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public class UserProcess : IDisposable
    {
        public static UserProcess Run(
            IFile file,
            string arguments,
            IDirectory workingDirectory,
            string username,
            string password)
        {
            var outputFile = workingDirectory.File(file.Name() + ".output.log");
            if (outputFile.Exists())
            {
                outputFile.Delete();
            }
            string command = "cmd /c {0} {1} > {2} 2>&1".FormatFrom(
                file.Path, arguments, outputFile);
            
            outputFile.WriteAllText(command + Environment.NewLine);
            var process = Win32ProcessAsUser.CreateProcessWithLogon(command, Environment.UserDomainName, username, password);
            var shellProcess = new UserProcess(process, outputFile);
            return shellProcess;
        }

        private readonly Process _process;
        private readonly IFile _outputFile;

        private UserProcess(Process process, IFile outputFile)
        {
            _process = process;
            _outputFile = outputFile;
        }

        public Process Process
        {
            get { return _process; }
        }

        public IFile OutputFile
        {
            get { return _outputFile; }
        }

        public void Dispose()
        {
            bool running = false;
            try
            {
                running = !_process.HasExited;
            }
            catch{}

            if (running)
            {
                _process.Kill();
            }
            _process.WaitForExit(1000);
            _process.Dispose();
        }
    }
}
