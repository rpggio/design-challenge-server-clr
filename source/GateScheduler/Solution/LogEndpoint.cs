using System;
using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    public class LogEndpoint : NancyModule
    {
        public LogEndpoint()
            : base("/log")
        {
            Post["/"] = _ =>
            {
                var request = this.Bind<LogRequest>();
                Console.WriteLine(request.Message);
                return null;
            };
        }
    }

    public class LogRequest
    {
        public string Message { get; set; }
    }
}
