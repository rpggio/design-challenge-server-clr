using System;
using System.Threading;
using DCS.Contracts;
using DCS.Core;
using DCS.ServerRuntime.Services;
using log4net;
using Rebus;

namespace DCS.Services.Bus
{
    public class BuildCleaner : IHandleMessages<DeleteDirectory>
    {
        private readonly ILog _log;
        private readonly AppSettings _settings;
        private readonly Shell _shell;

        public BuildCleaner(ILog log, AppSettings settings, Shell shell)
        {
            _log = log;
            _settings = settings;
            _shell = shell;
        }

        public void Handle(DeleteDirectory message)
        {
            string path = message.Path;
            if (!path.ContainsIgnoreCase(_settings.Env.BuildsDirectory))
            {
                _log.WarnFormat("Rejecting request to delete [{0}]", path);
                return;
            }

            try
            {
                _shell.RunBackground("cmd",
                    string.Format("/c rmdir /s /q \"{0}\"", path),
                    _settings.Env.BuildsDirectory);
            }
            catch (Exception ex)
            {
                _log.InfoFormat("Failed to delete {0}: {1}", path, ex.Summary());
                // Wait for a big to reject/retry. Yes, blocking :(
                Thread.Sleep(5000);
            }
        }
    }
}