using System.IO;
using DCS.Core;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services;
using log4net;

namespace DCS.ServerRuntime.Bootstrap
{
    public class CheckRubyEnvOperation : IOperation
    {
        private readonly AppSettings _settings;
        private readonly Shell _shell;
        private readonly ILog _log;

        public CheckRubyEnvOperation(AppSettings settings, Shell shell, ILog log)
        {
            _settings = settings;
            _shell = shell;
            _log = log;
        }

        public void Execute()
        {
            var ruby = Path.Combine(_settings.Env.RubyBin, "ruby.exe");
            _log.InfoFormat("Checking Ruby at {0}", ruby);
            int result = _shell.RunBackground(
                ruby,
                "--version");
            if (result == 0)
            {
                _log.InfoFormat("Verified Ruby env");
            }
            else
            {
                _log.ErrorFormat("Could not run Ruby at {0}", ruby);
            }
        }
    }
}