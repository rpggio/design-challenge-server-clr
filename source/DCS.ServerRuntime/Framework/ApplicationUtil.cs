using System;
using System.IO;
using DCS.Core;

namespace DCS.ServerRuntime.Framework
{
    public static class ApplicationUtil
    {
        public static string GetConfigDirectory()
        {
            string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (string.IsNullOrEmpty(configPath))
            {
                throw new Exception("Could not find config path for application");
            }

            var configDir = Path.GetDirectoryName(configPath);
            if (configDir.IsEmpty() || !Directory.Exists(configDir))
            {
                throw new Exception("No config directory found for {0}".FormatFrom(configPath));
            }
            
            return configDir;
        }
    }
}
