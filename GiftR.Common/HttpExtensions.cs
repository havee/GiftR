using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;

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

        public static Uri GetBaseUrl(this UrlHelper url)
        {
            Uri contextUri = new Uri(url.RequestContext.HttpContext.Request.Url, url.RequestContext.HttpContext.Request.RawUrl);
            UriBuilder realmUri = new UriBuilder(contextUri) { Path = url.RequestContext.HttpContext.Request.ApplicationPath, Query = null, Fragment = null };

            return realmUri.Uri;
        }

        public static Uri GetCompleteUrl(this UrlHelper url, string relativePath)
        {
            Uri contextUri = new Uri(url.RequestContext.HttpContext.Request.Url, url.RequestContext.HttpContext.Request.RawUrl);

            if (relativePath.StartsWith(@"/")) relativePath = relativePath.Substring(1);

            UriBuilder realmUri = new UriBuilder(contextUri) { Path = url.RequestContext.HttpContext.Request.ApplicationPath + relativePath, Query = null, Fragment = null };

            return realmUri.Uri;
        }  
    }
}