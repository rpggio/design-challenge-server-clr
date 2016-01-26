using System;
using log4net;

namespace DCS.ServerRuntime.Framework
{
    public static class OperationsUtil
    {
        public static void ExecuteSafe(this IOperation operation, ILog log)
        {
            try
            {
                operation.Execute();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error executing {0}", operation), ex);
            }
        }
    }
}