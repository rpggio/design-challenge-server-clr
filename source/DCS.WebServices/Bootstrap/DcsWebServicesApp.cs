using System;
using System.Linq;
using Autofac;
using DCS.Core;
using DCS.ServerRuntime.Bootstrap;
using DCS.ServerRuntime.Bus;
using DCS.ServerRuntime.Services;
using DCS.WebServices.Api;
using log4net;

namespace DCS.WebServices.Bootstrap
{
    public class DcsWebServicesApp
    {
        private static IContainer _container;

        public static IContainer Container
        {
            get { return _container; }
        }

        //private IBus _bus;
        private DcsWebServicesApi _api;

        public void Start()
        {
            _container = InitializeContainer();
            var log = _container.Resolve<ILog>();

            // check settings
            var settings = _container.Resolve<AppSettings>();
            log.Info(settings.Values.Select(kvp => "  {0}: {1}".FormatFrom(kvp.Key, kvp.Value))
                         .JoinString(Environment.NewLine));

            log.Info("Starting API host");
            _api = _container.Resolve<DcsWebServicesApi>();
            _api.Start();
        }

        public void Stop()
        {
            FuncUtil.TryExecute(_api.Dispose);
        }

        private static IContainer InitializeContainer()
        {
            var builder = new ContainerBuilder();
            const string appName = "dcs.webServices";

            builder.RegisterLog4Net(appName);
            builder.RegisterServerRuntimeComponents();

            builder.RegisterDb();
            builder.RegisterEntityStorage();
            
            builder.RegisterType<DcsWebServicesApi>();

            var container = builder.Build();

            var bus = container.ReGisterReBusBus(new AppSettings(), appName);
            container.RegisterRebusHandlersFromAssembly(bus, typeof(DcsWebServicesApp).Assembly);
            return container;
        }
    }
}