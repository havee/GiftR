using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftR.WebUI.Code
{
    public static class HttpExtensions
    {
        public static void RedirectWithQueryString(this HttpResponse response, string url)
        {
            var server = HttpContext.Current.Server;
            var request = HttpContext.Current.Request;

            var qs = server.UrlEncode(request.QueryString.ToString());

            response.Redirect(url + "?" + server.UrlDecode(qs));
        }
    }
}