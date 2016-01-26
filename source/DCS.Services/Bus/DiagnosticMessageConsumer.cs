using System;
using DCS.Contracts;
using log4net;
using Rebus;

namespace DCS.Services.Bus
{
    public class DiagnosticMessageConsumer
        : IHandleMessages<ThrowExceptionMessage>
    {
        private readonly ILog _log;

        public DiagnosticMessageConsumer(ILog log)
        {
            _log = log;
        }

        public void Handle(ThrowExceptionMessage message)
        {
            var exception = new Exception(message.Message);
            _log.Error("testing error", exception);
            throw exception;
        }
    }
}