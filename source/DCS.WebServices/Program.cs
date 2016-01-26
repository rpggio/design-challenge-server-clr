using System.IO;
using DCS.ServerRuntime.Framework;
using DCS.WebServices.Bootstrap;
using log4net.Config;
using Topshelf;

namespace DCS.WebServices
{
    class Program
    {
        static void Main(string[] args)
        {
            string logConfigPath = Path.Combine(
                ApplicationUtil.GetConfigDirectory(),
                "dcs.webServices.log4net.config");
            XmlConfigurator.Configure(new FileInfo(logConfigPath));

            HostFactory.Run(configure =>
            {
                configure.Service<DcsWebServicesApp>(s =>
                {
                    s.ConstructUsing(name => new DcsWebServicesApp());
                    s.WhenStarted(sh => sh.Start());
                    s.WhenStopped(sh => sh.Stop());
                });
                configure.RunAsNetworkService();

                configure.SetDescription("CodeFlight Web Services Host");
                configure.SetDisplayName("DCS.WebServices");
                configure.SetServiceName("DCS.WebServices");
            });
        }
    }
}
