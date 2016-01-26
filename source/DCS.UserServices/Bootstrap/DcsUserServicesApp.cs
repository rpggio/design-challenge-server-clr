using System;
using System.Linq;
using Autofac;
using DCS.Core;
using DCS.ServerRuntime.Bootstrap;
using DCS.ServerRuntime.Bus;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services;
using log4net;
using Rebus;

namespace DCS.UserServices.Bootstrap
{
    public class DcsUserServicesApp
    {
        private IBus _bus;

        public void Start()
        {
            var container = InitializeContainer();

            var log = container.Resolve<ILog>();
            var settings = container.Resolve<AppSettings>();

            log.Info("Staring dcs.userServices app");

            settings.Env.Validate();
            log.Info(settings.Values.Select(kvp => "  {0}: {1}".FormatFrom(kvp.Key, kvp.Value))
                         .JoinString(Environment.NewLine));

            container.Resolve<CheckRubyEnvOperation>().ExecuteSafe(log);
            container.Resolve<KillOrphansOperation>().ExecuteSafe(log);
            
            _bus = container.Resolve<IBus>();
        }

        public void Stop()
        {
            FuncUtil.TryExecute(_bus.Dispose);

            //if (AppContainer.Instance != null)
            //{
            //    var killChildren = AppContainer.Instance.Resolve<KillChildrenOperation>();
            //    killChildren.Execute();
            //}
        }

        private static IContainer InitializeContainer()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(DcsUserServicesApp).Assembly;
            const string appName = "dcs.userServices";

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