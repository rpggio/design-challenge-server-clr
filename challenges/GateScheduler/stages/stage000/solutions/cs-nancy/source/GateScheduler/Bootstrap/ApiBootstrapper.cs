using System.Globalization;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GateScheduler.Bootstrap
{
    /// <summary>
    /// This customizes the behavior of the NancyFX embedded web server.
    /// </summary>
    public class ApiBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // call default base configuration
            base.ConfigureApplicationContainer(container);

            // register JSON serializer customizations
            var serializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            serializer.Converters.Add(new HoursMinutesNullableTimeSpanConverter());
            container.Register(serializer);

            // register application types in DI container
            GateSchedulerRegistrations.Register(container);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            // Send JSON message on error.
            // This works in conjunction with ApiErrorHandler.
            pipelines.OnError.AddItemToEndOfPipeline((c, ex) =>
                new JsonResponse(new {
                    errorType = ex.GetType(),
                    errorMessage = ex.Message,
                    stackTrace = ex.StackTrace.ToString(CultureInfo.InvariantCulture)
                },
                new DefaultJsonSerializer()) {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            base.RequestStartup(container, pipelines, context);
        }
    }
}
