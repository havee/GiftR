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

namespace GiftR.MVCWeb.Controllers
{
    public class SitesController : Controller
    {

        // GET: /Sites/Polaroid
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

            return View(query.ToList());
        }

        void Photos_OnError(ProviderException providerException)
        {
            // Logging.Logger.Error(providerException.Message, providerException);
        }  
    }
}
