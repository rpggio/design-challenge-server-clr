using System;
using DCS.Core;
using DCS.ServerRuntime.Framework;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent]
    public class DcsHrefs
    {
        private readonly AppSettings _appSettings;

        public DcsHrefs(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string UserPage(Guid userId)
        {
            return "{0}/user.html#?userId={1}".FormatFrom(_appSettings.Env.WebsiteHref, userId);
        }
    }
}