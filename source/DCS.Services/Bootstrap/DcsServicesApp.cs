using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using DCS.Core;
using DCS.ServerRuntime.Bootstrap;
using DCS.ServerRuntime.Bus;
using DCS.ServerRuntime.Services;
using log4net;
using Rebus;

namespace DCS.Services.Bootstrap
{
    public class DcsServicesApp
    {
        private IBus _bus;

        internal static IContainer Container { get; set; }

        public void Start()
        {
            Container = InitializeContainer();

            var log = Container.Resolve<ILog>();
            var settings = Container.Resolve<AppSettings>();

            log.Info("Staring dcs.userServices app");

            settings.Env.Validate();
            log.Info(settings.Values.Select(kvp => "  {0}: {1}".FormatFrom(kvp.Key, kvp.Value))
                .JoinString(Environment.NewLine));

            if (Debugger.IsAttached)
            {
                var initDatabase = Container.Resolve<InitializeDatabaseOperation>();
                initDatabase.Execute();
            }

            log.Info("Starting service bus");
            _bus = Container.Resolve<IBus>();
        }

        public void Stop()
        {
            FuncUtil.TryExecute(_bus.Dispose);
        }

        private static IContainer InitializeContainer()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof (DcsServicesApp).Assembly;
            const string appName = "dcs.services";

            builder.RegisterLog4Net(appName);
            builder.RegisterServerRuntimeComponents();

            builder.RegisterDb();
            builder.RegisterEntityStorage();

            var container = builder.Build();

            var bus = container.ReGisterReBusBus(new AppSettings(), appName);
            container.RegisterRebusHandlersFromAssembly(bus, assembly);
            return container;
        }
    }
}