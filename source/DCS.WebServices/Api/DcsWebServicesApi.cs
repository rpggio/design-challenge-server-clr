using System;
using DCS.ServerRuntime.Services;
using log4net;
using Nancy.Hosting.Self;

namespace DCS.WebServices.Api
{
    public sealed class DcsWebServicesApi : IDisposable
    {
        private readonly AppSettings _settings;
        private ILog _log;
        private NancyHost _host;

        public DcsWebServicesApi(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        public void Start()
        {
            var address = _settings.Env.HostWebAtAddress;
            _host = new NancyHost(new Uri(address));
            _host.Start();
            _log.InfoFormat("Web server started at {0}", address);
        }

        public void Dispose()
        {
            if (_host == null) return;
            _host.Dispose();
            _host = null;
        }
    }
}