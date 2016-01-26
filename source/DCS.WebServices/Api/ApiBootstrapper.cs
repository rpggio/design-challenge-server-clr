using System.Globalization;
using Autofac;
using DCS.WebServices.Bootstrap;
using Nancy;
using Nancy.Authentication.Token;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DCS.WebServices.Api
{
    public class ApiBootstrapper : AutofacNancyBootstrapper
    {
        protected override ILifetimeScope GetApplicationContainer()
        {
            return DcsWebServicesApp.Container.BeginLifetimeScope(builder =>
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };
                serializer.Converters.Add(new StringEnumConverter
                {
                    AllowIntegerValues = false,
                    CamelCaseText = true
                });
                builder.RegisterInstance(serializer);

                builder.RegisterInstance<ITokenizer>(new Tokenizer());
            });
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            TokenAuthentication.Enable(pipelines, 
                new TokenAuthenticationConfiguration(container.Resolve<ITokenizer>()));

            pipelines.OnError.AddItemToEndOfPipeline((c, ex) =>
                new JsonResponse(new
                {
                    errorType = ex.GetType(),
                    errorMessage = ex.Message,
                    stackTrace = ex.StackTrace.ToString(CultureInfo.InvariantCulture)
                },
                    new DefaultJsonSerializer())
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // base startup
            base.RequestStartup(container, pipelines, context);

            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type, Authorization"));
        }
    }
}