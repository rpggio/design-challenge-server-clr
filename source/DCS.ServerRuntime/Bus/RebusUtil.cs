using System;
using System.Linq;
using System.Reflection;
using Autofac;
using DCS.ServerRuntime.Services;
using Rebus;
using Rebus.Autofac;
using Rebus.Configuration;
using Rebus.Log4Net;
using Rebus.Transports.Sql;

namespace DCS.ServerRuntime.Bus
{
    public static class RebusUtil
    {
        public static void RegisterRebusHandlersFromAssembly(this IContainer container, IBus bus, Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof (IHandleMessages<>));
            builder.Update(container);

            foreach (var type in assembly.GetTypes())
            {
                var handlerIterfaceTypes = type.GetInterfaces().Where(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>));
                foreach (var interfaceType in handlerIterfaceTypes)
                {
                    bus.Advanced.Routing.Subscribe(interfaceType.GenericTypeArguments[0]);
                }
            }
        }
        
        public static IBus ReGisterReBusBus(
            this IContainer container, 
            AppSettings settings, 
            string applicationName)
        {
            string inputQueue = applicationName + ".input";
            string errorQueue = applicationName + ".error";

            var adapter = new AutofacContainerAdapter(container);
            var bus = Configure.With(adapter)
                .Logging(l => l.Log4Net())
                .Transport(t => t.UseSqlServer(settings.Env.DcsDbConn,
                    "BusTransport",
                    inputQueue,
                    errorQueue)
                    .EnsureTableIsCreated())
                .Serialization(s => s.UseJsonSerializer())
                .MessageOwnership(m => m.Use(new SingleDestinationMessageOwnership(inputQueue)))
                .Subscriptions(s =>
                    s.StoreInSqlServer(settings.Env.DcsDbConn, "BusSubscription")
                        .EnsureTableIsCreated())
                .Behavior(b => b.SetMaxRetriesFor<Exception>(1))
                .CreateBus()
                .Start();

            return bus;
        }
    }
}
