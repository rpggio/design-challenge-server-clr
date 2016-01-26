using System;
using System.Net;

namespace DCS.Core.Net
{
    public class WebClientExt : WebClient
    {
        public TimeSpan Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = (int) Timeout.TotalMilliseconds;
            }

            return request;
        }
    }
}
