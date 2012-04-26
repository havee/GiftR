using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotNetOpenAuth.ApplicationBlock.Facebook;
using GiftR.Common;
using GiftR.Services;
using DotNetOpenAuth.ApplicationBlock;
using System.Configuration;
using System.Net;
using DotNetOpenAuth.OAuth2;

namespace GiftR.MVCWeb.Controllers
{
    [Authorize()]
    public class HomeController : Controller
    {
        private ISitesService sitesService;

        public HomeController(ISitesService service)
        {
            this.sitesService = service;
        }

        public void Index(string icode)
        {
            if (!String.IsNullOrEmpty(icode))
            {
                var site = sitesService.GetSiteByCode(icode);
                if (site != null)
                {
                    StateManager.AddSite(site);

                    ViewSite(site.SitesTypes.default_page);
                }
            }
            else
            {
                CreateSite();
            }
        }

        public void ViewSite(string page)
        {
            page += "/" + RouteData.Values["icode"];

            Response.Redirect(page);
        }

        public ActionResult CreateSite()
        {
            return View();
        }

        
    }
}
