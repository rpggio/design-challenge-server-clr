using System;
using System.Threading;
using GateScheduler.Bootstrap;
using GateScheduler.Solution;
using Nancy.Hosting.Self;

namespace GateScheduler
{
    class Program
    {
        internal static string UserKey { get; set; }

        static int Main(string[] args)
        {
            if (args.Length == 0 ||
                string.IsNullOrEmpty(UserKey = args[0]))
            {
                Console.WriteLine("userKey argument is required");
                return 1;
            }

            const int port = 40001;
            var address = string.Format("http://localhost:{0}", port);

            var hostConfig = new HostConfiguration
            {
                UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true
                }
            };

            using (var host = new NancyHost(hostConfig, new Uri(address)))
            {
                host.Start();
                var scheduler = ApiBootstrapper.Container.Resolve<Scheduler>();

                Console.WriteLine("started {0}", port);

                while (scheduler.State != SystemState.Finished)
                {
                    Thread.Sleep(1000);
                }
            }

            return 0;
        }
    }
}
