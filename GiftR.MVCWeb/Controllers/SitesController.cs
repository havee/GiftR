using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GiftR.Services;

using Linq.Flickr;
using Linq.Flickr.Authentication;
using Linq.Flickr.Configuration;
using Linq.Flickr.Repository;
using Linq.Flickr.Authentication.Providers;
using Linq.Flickr.Proxies;
using Linq.Flickr.Repository.Abstraction;
using LinqExtender;
using GiftR.MVCWeb.Models;
using GiftR.Model;

using System.Data.Entity;
using GiftR.Common;
using System.Web.Routing;
using DotNetOpenAuth.ApplicationBlock;

namespace GiftR.MVCWeb.Controllers
{
    [Authorize]
    public class SitesController : Controller
    {

        // GET: /Sites/Polaroid
        [OutputCache(Duration = 30, VaryByParam = "icode")]
        public ActionResult Polaroid(string icode)
        {
            var site = SitesService.GetSiteByCode(icode);

            FlickrContext context = new FlickrContext();
            context.Photos.OnError += new Query<Photo>.ErrorHandler(Photos_OnError);

            var bigs = (from ph in context.Photos
                        where ph.User == site.flickr_username
                        && ph.PhotoSize == PhotoSize.Default
                        select ph).ToList();

            var thumbs = (from ph in context.Photos
                          where ph.User == site.flickr_username
                          && ph.PhotoSize == PhotoSize.Thumbnail
                          select ph).ToList();

            var query = from b in bigs
                        join t in thumbs on b.Id equals t.Id
                        select new Image() { Src = t.Url, Alt = b.Url, Id = t.Id, Title = t.Title };


            ViewBag.Title = site.title;
            return View(query.ToList());
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SiteModel sitemodel)
        {
            if (ModelState.IsValid)
            {
                var id = Convert.ToInt32(StateManager.GetCurrentUser());
                var site = new Sites()
                {
                    flickr_username = sitemodel.FlickrUserName,
                    title = sitemodel.Title,
                    site_type = sitemodel.SiteType,
                    verification_code = Guid.NewGuid().ToString()
                };

                SitesService.Save(id, sitemodel.Email, site);

                var shortUrl = GenerateSiteShortLink(site.verification_code);
                
                return RedirectToAction("CreateSuccess", new RouteValueDictionary 
            { 
                { 
                    "msg", 
                    "El sitio se ha creado satisfactoriamente, accedé ya al mismo:"
                }
                ,
                {
                    "url",
                    shortUrl
                }
            });
            }

            return View();
        }

        public string GenerateSiteShortLink(string verification_code)
        {
            var action = Url.Action("Index", "Home", new RouteValueDictionary { { "icode", verification_code } });
            if (action.StartsWith(@"/"))
            {
                action = action.Substring(1);
            }

            var url = Url.GetBaseUrl() + action;
            var shortUrl = GoogleConsumer.ShortenUrl(url);
            
            return shortUrl;
        }

        public ActionResult CreateSuccess(string msg, string url)
        {            
            ViewBag.Msg = msg;
            ViewBag.Url = url;

            return View();
        }

        public ActionResult MySites()
        {
            var query = SitesService.GetSiteById(Convert.ToInt32(Common.StateManager.GetCurrentUser()));
            var sites = from p in query
                        select
                        new SiteModel()
                        {
                            Email = p.SitesOwners.First().email,
                            FlickrUserName = p.flickr_username,
                            Id = p.id,
                            SiteType = p.site_type,
                            Title = p.title,
                            Url = GenerateSiteShortLink(p.verification_code),
                            Verification_Code = p.verification_code
                        };


            return View(sites.ToList());
        }

        public ActionResult Delete(int id)
        {
            SitesService.Delete(id);

            return RedirectToAction("MySites");
        }

        void Photos_OnError(ProviderException providerException)
        {
            // Logging.Logger.Error(providerException.Message, providerException);
        }  
    }
}
