using DCS.ServerRuntime.Services;
using Nancy;

namespace DCS.WebServices.Api
{
    public class SettingsEndpoint : NancyModule
    {
        private readonly AppSettings _settings;

        public SettingsEndpoint(AppSettings settings)
            : base("/settings")
        {
            _settings = settings;

            Get["/"] = _ => _settings;
        }
    }
}