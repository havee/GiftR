using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace GiftR.Common
{
    public static class HttpExtensions
    {
        public static void RedirectWithQueryString(this HttpResponseBase response, string url)
        {
            var server = HttpContext.Current.Server;
            var request = HttpContext.Current.Request;

            var qs = server.UrlEncode(request.QueryString.ToString());

            response.Redirect(url + "?" + server.UrlDecode(qs));
        }


        
    }
}