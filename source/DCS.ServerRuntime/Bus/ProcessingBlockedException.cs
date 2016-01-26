using System;

namespace DCS.ServerRuntime.Bus
{
    public class ProcessingBlockedException : Exception
    {
        public ProcessingBlockedException()
        {
        }

        public ProcessingBlockedException(string message) : base(message)
        {
        }

        public ProcessingBlockedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}