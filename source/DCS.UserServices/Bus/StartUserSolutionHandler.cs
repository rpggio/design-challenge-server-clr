using System.Diagnostics;
using System.IO;
using DCS.Contracts;
using DCS.Core;
using log4net;
using Rebus;

namespace DCS.UserServices.Bus
{
    public class StartUserSolutionHandler : IHandleMessages<StartUserSolution>
    {
        private readonly ILog _log;
        private readonly Shell _shell;

        public StartUserSolutionHandler(Shell shell, ILog log)
        {
            _shell = shell;
            _log = log;
        }

        public void Handle(StartUserSolution message)
        {
            _log.InfoFormat("Starting user solution: {0} {1}", message.SolutionPath, message.SolutionArgs);
            _shell.StartBackgroundProcess(
                new ProcessStartInfo(message.SolutionPath, message.SolutionArgs)
                {
                    WorkingDirectory = Path.GetDirectoryName(message.SolutionPath)
                },
                OnOutput,
                OnError);
        }

        private void OnError(string obj)
        {
            _log.Info(obj);
        }

        private void OnOutput(string obj)
        {
            _log.Info(obj);
        }
    }
}