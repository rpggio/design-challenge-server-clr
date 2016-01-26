using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses;

namespace GateScheduler.Bootstrap
{
    /// <summary>
    /// Prints API errors in JSON instead of HTML.
    /// This type is automatically picked up by NancyFX.
    /// </summary>
    public class ApiErrorHandler : IStatusCodeHandler
    {
        private readonly IEnumerable<ISerializer> _serializers;

        public ApiErrorHandler(IEnumerable<ISerializer> serializers)
        {
            _serializers = serializers;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                || statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (statusCode == HttpStatusCode.NotFound)
            {
                context.Response = new JsonResponse(
                    new {error = "path not found"},
                    _serializers.First(s => s.CanSerialize("application/json")));
            }

            context.Response.StatusCode = statusCode;
        }
    }
}
