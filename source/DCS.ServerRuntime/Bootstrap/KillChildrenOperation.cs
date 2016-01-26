using System;
using System.Diagnostics;
using System.Management;
using DCS.Core;
using DCS.ServerRuntime.Framework;
using log4net;

namespace DCS.ServerRuntime.Bootstrap
{
    public class KillChildrenOperation : IOperation
    {
        private readonly ILog _log;

        public KillChildrenOperation(ILog log)
        {
            _log = log;
        }

        public void Execute()
        {
            _log.InfoFormat("Checking for running child processes");

            int currentProcessId = Process.GetCurrentProcess().Id;
            var searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + currentProcessId);
            var collection = searcher.Get();
            foreach (var item in collection)
            {
                // todo: check for process owner

                UInt32 childProcessId = (UInt32)item["ProcessId"];
                if ((int)childProcessId != currentProcessId)
                {
                    var childProcess = Process.GetProcessById((int)childProcessId);
                    _log.InfoFormat("Killing child process {0} [{1}]", childProcess.ProcessName, childProcessId);
                    try
                    {
                        childProcess.Kill();
                    }
                    catch (Exception ex)
                    {
                        _log.WarnFormat("Failed to kill child process {0} [{1}]: {2}", 
                            childProcess.ProcessName, 
                            childProcessId,
                            ex.Summary());
                    }
                }
            }
        }
    }
}