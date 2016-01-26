using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using DCS.Console.Commands;
using DCS.Core;
using DCS.ServerRuntime.Bootstrap;
using DCS.ServerRuntime.Bus;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services;
using log4net;
using log4net.Config;

namespace DCS.Console
{
    internal class Program
    {
        internal const string DefaultChallenge = "GateScheduler";

        internal static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            string logConfigPath = Path.Combine(
                ApplicationUtil.GetConfigDirectory(),
                "dcs.console.log4net.config");
            XmlConfigurator.Configure(new FileInfo(logConfigPath));

            System.Console.WriteLine();

            using (var container = Container = InitializeContainer())
            {
                var log = container.Resolve<ILog>();
                log.InfoFormat("started dcs.console {0}", args.JoinString(" "));

                var settings = container.Resolve<AppSettings>();
                settings.Env.Validate();
                log.Info(settings.Values.Select(kvp => "  {0}: {1}".FormatFrom(kvp.Key, kvp.Value))
                    .JoinString(Environment.NewLine));

                var commandTypes = typeof (MessageCommandBase).Assembly
                    .GetTypes().Where(t => t.IsAssignableTo<IConsoleCommand>() && t.IsClass && !t.IsAbstract)
                    .ToDictionary(t => Regex.Replace(t.Name, "Command$", ""),
                        StringComparer.OrdinalIgnoreCase);

                Action<int, string> usageAndExit = (exitCode, message) =>
                {
                    if (message != null)
                    {
                        System.Console.WriteLine(message);
                        System.Console.WriteLine();
                    }
                    System.Console.WriteLine("usage: dcs.console <command> <command args>");
                    System.Console.WriteLine("commands: {0}", commandTypes.Keys.JoinString(", "));
                    Environment.Exit(exitCode);
                };

                if (args.Length < 1)
                {
                    usageAndExit(1, null);
                }

                var argQueue = new Queue<string>(args);
                string commandName = argQueue.Dequeue();
                Type commandType = null;
                if (!commandTypes.TryGetValue(commandName, out commandType))
                {
                    usageAndExit(1, "invalid command: {0}".FormatFrom(commandName));
                }

                using (var scope = container.BeginLifetimeScope(c => c.RegisterType(commandType)))
                {
                    var command = (IConsoleCommand) scope.Resolve(commandType);
                    string message;
                    if (!command.TryParse(argQueue.ToArray(), out message))
                    {
                        command.PrintHelp(new IndentedTextWriter(System.Console.Out));
                        System.Console.WriteLine();
                        usageAndExit(1, message);
                    }

                    command.Execute();
                }
            }
        }

        private static IContainer InitializeContainer()
        {
            const string appName = "dcs.console";
            var builder = new ContainerBuilder();

            builder.RegisterLog4Net(appName);
            builder.RegisterServerRuntimeComponents();

            builder.RegisterDb();
            builder.RegisterEntityStorage();

            var container = builder.Build();

            var assembly = typeof (Program).Assembly;
            var bus = container.ReGisterReBusBus(new AppSettings(), appName);
            container.RegisterRebusHandlersFromAssembly(bus, assembly);
            return container;

        }
    }
}