using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace GiftR.WebUI.Code
{
    public static class PageFlowExtensions
    {
        public static Uri GetDefaultPageUrl()
        {
            var url = ConfigurationManager.AppSettings["baseUrl"] + ConfigurationManager.AppSettings["defaultPage"];

            return new Uri(url);
        }

        public static Uri GetCreateSitePageUrl()
        {
            var url = ConfigurationManager.AppSettings["baseUrl"] + ConfigurationManager.AppSettings["createSitePage"];

            return new Uri(url);
        }
    }
}