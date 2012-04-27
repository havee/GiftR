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

        public ActionResult Index(string icode)
        {
            if (!String.IsNullOrEmpty(icode))
            {
                var site = sitesService.GetSiteByCode(icode);
                if (site != null)
                {
                    StateManager.AddSite(site);

                    return ViewSite(site.SitesTypes.default_page);
                }
            }
            else
            {
                return RedirectToAction("CreateSite");
            }

            return View();
        }

        public ActionResult ViewSite(string page)
        {
            page += "/" + RouteData.Values["icode"];

            return RedirectPermanent(page);            
        }

        public ActionResult CreateSite()
        {
            return View();
        }

        
    }
}
