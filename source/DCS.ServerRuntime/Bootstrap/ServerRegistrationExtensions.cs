using System;
using System.Data;
using Autofac;
using DCS.Core;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services;
using log4net;
using Magnum.Extensions;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace DCS.ServerRuntime.Bootstrap
{
    public static class ServerRegistrationExtensions
    {
        public static void RegisterServerRuntimeComponents(this ContainerBuilder builder)
        {
            builder.RegisterDeclaredComponentsFrom(typeof (ServerRegistrationExtensions).Assembly);

            builder.Register(c =>
            {
                var log = c.Resolve<ILog>();
                return new Shell(log.Debug, log.Debug);
            });
        }

        public static void RegisterLog4Net(this ContainerBuilder builder, string appName)
        {
            var logger = LogManager.GetLogger(appName);
            logger.InfoFormat("Registering logger {0}", appName);
            builder.RegisterInstance(logger).As<ILog>();
        }

        public static void RegisterDb(this ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var settings = c.Resolve<AppSettings>();
                var factory = new OrmLiteConnectionFactory(settings.Env.DcsDbConn,
                    settings.Env.IsDcsDbSqlite ? SqliteDialect.Provider : SqlServerDialect.Provider);
                return factory;
            })
                .As<IDbConnectionFactory>()
                .InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var db = c.Resolve<IDbConnectionFactory>().OpenDbConnection();
                //db.ExecuteSql("PRAGMA foreign_keys = ON");    // may need this for Sqlite someday
                return db;
            })
            .As<IDbConnection>();
        }

        public static void RegisterEntityStorage(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EntityStore<,>));

            builder.RegisterAssemblyTypes(typeof(EntitiesRoot).Assembly)
                .Where(t => t.Implements<IOperation>() || t.Implements<Entities.IEntityStore>())
                .AsSelf();

            builder.RegisterType<EntitiesRoot>().InstancePerLifetimeScope();
        }
    }
}