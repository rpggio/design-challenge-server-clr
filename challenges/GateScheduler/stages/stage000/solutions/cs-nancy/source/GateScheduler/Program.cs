using System;
using System.Threading;
using Nancy.Hosting.Self;

namespace GateScheduler
{
    public class Program
    {
        /// <summary>
        /// Default HTTP port used for hosting locally.
        /// The first time this application runs, it will ask to elevate permissions
        /// in order to register this port for use.
        /// </summary>
        public const int TestingPort = 40000;

        internal static string StartupKey { get; set; }

        static void Main(string[] args)
        {
            int port = args.Length > 0
                ? int.Parse(args[0])
                : TestingPort;
            StartupKey = args.Length > 1
                ? args[1]
                : null;

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

                Console.WriteLine("started {0} {1}", port, StartupKey);

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
