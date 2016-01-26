using Autofac;
using DCS.ServerRuntime.Bootstrap;
using Xunit.Ioc;
using Xunit.Ioc.Autofac;

namespace DCS.UnitTests
{
    public class AutofacContainerBootstrapper : IDependencyResolverBootstrapper
    {
        private static readonly object _lock = new object();
        private static IDependencyResolver _dependencyResolver;

        public IContainer CreateContainer()
        {
            const string appName = "dcs.unitTests";

            var builder = new ContainerBuilder();
            
            builder.RegisterLog4Net(appName);
            builder.RegisterServerRuntimeComponents();

            builder.RegisterDb();
            builder.RegisterEntityStorage();

            builder.RegisterAssemblyTypes(typeof (AutofacContainerBootstrapper).Assembly);
            return builder.Build();
        }

        public IDependencyResolver GetResolver()
        {
            lock (_lock)
            {
                if (_dependencyResolver == null)
                    _dependencyResolver = new AutofacDependencyResolver(CreateContainer());
                return _dependencyResolver;
            }
        }
    }
}