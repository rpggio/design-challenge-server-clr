using System.IO;
using DCS.ServerRuntime.Framework;
using DCS.Services.Bootstrap;
using log4net.Config;
using Topshelf;

namespace DCS.Services
{
    internal class Program
    {
        public static void Main()
        {
            string logConfigPath = Path.Combine(
                ApplicationUtil.GetConfigDirectory(),
                "dcs.services.log4net.config");
            XmlConfigurator.Configure(new FileInfo(logConfigPath));

            HostFactory.Run(configure =>
            {
                configure.Service<DcsServicesApp>(s =>
                {
                    s.ConstructUsing(name => new DcsServicesApp());
                    s.WhenStarted(sh => sh.Start());
                    s.WhenStopped(sh => sh.Stop());
                });
                configure.RunAsLocalSystem();

                configure.SetDescription("CodeFlight Design Services Host");
                configure.SetDisplayName("DCS.Services");
                configure.SetServiceName("DCS.Services");
            });
        }
    }
}