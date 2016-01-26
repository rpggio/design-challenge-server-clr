using System;
using DCS.ServerRuntime.Bootstrap;
using DCS.ServerRuntime.Services;
using DCS.UserServices.Bootstrap;
using ServiceStack;
using Topshelf;

namespace DCS.UserServices
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(configure =>
            {
                configure.Service<DcsUserServicesApp>(s =>
                {
                    s.ConstructUsing(name => new DcsUserServicesApp());
                    s.WhenStarted(sh => sh.Start());
                    s.WhenStopped(sh => sh.Stop());
                });

                var settings = new AppSettings();
                configure.MustUseLog4Net("dcs.userServices.log4net.config");
                configure.RunAs(
                    @"{0}\{1}".FormatWith(Environment.MachineName, settings.Env.SolutionExecUsername), 
                    settings.Env.SolutionExecPassword);

                configure.SetDescription("CodeFlight Design Services User Host");
                configure.SetDisplayName("DCS.UserServices");
                configure.SetServiceName("DCS.UserServices");
            });
        }
    }
}
