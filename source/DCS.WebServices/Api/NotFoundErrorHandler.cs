using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses;

namespace DCS.WebServices.Api
{
    public class NotFoundErrorHandler : IStatusCodeHandler
    {
        private readonly IEnumerable<ISerializer> _serializers;

        public NotFoundErrorHandler(IEnumerable<ISerializer> serializers)
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