using System;
using System.Configuration;
using System.IO;
using DCS.Core;
using Topshelf;
using Topshelf.HostConfigurators;

namespace DCS.ServerRuntime.Bootstrap
{
    public static class TopShelfExtensions
    {
        public static void MustUseLog4Net(this HostConfigurator configurator, string configFileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
            if (!File.Exists(path))
            {
                throw new ConfigurationException("Logging config file does not exist: {0}".FormatFrom(path));
            }
            configurator.UseLog4Net(configFileName);
        }
    }
}
