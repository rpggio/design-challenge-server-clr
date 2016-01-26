using System;
using System.Configuration;
using System.Diagnostics;
using System.Management;
using DCS.Core;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services;
using log4net;

namespace DCS.ServerRuntime.Bootstrap
{
    public class KillOrphansOperation : IOperation
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;

        public KillOrphansOperation(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        public void Execute()
        {
            if (_settings.Env.SolutionExecUsername.IsEmpty())
            {
                throw new ConfigurationErrorsException("Setting env-solutionExecUsername is empty");
            }

            _log.InfoFormat("Checking for orphaned processes");

            string[] propertiesToSelect = {"Handle", "ProcessId"};
            var processQuery = new SelectQuery("Win32_Process", "", propertiesToSelect);
            int currentProcessId = Process.GetCurrentProcess().Id;

            using (var searcher = new ManagementObjectSearcher(processQuery))
            using (var processes = searcher.Get())
            {
                foreach (var mbo in processes)
                {
                    try
                    {
                        var mo = (ManagementObject) mbo;
                        var outParameters = new object[2];
                        object result;
                        if (
                            FuncUtil.TryExecute(() => mo.InvokeMethod("GetOwner", outParameters), out result)
                            && ((uint) result) == 0
                            )
                        {
                            string user = (string) outParameters[0];
                            //string domain = (string) outParameters[1];
                            uint processId = (uint) mo["ProcessId"];

                            if (processId != currentProcessId
                                && _settings.Env.SolutionExecUsername.EqualsIgnoreCase(user))
                            {
                                var process = Process.GetProcessById((int) processId);
                                _log.InfoFormat("Killing orphan process {0} [{1}]", process.ProcessName, processId);
                                process.Kill();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Debug(string.Format("Error working with process {0}", mbo), ex);
                    }
                }
            }
        }
    }
}