using System;
using System.Net;

namespace SDH.MoviePlugins
{
    public class CustomWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = 15000; //15 Seconds
                request.KeepAlive = false;
            }
            return request;
        }
    }
}
