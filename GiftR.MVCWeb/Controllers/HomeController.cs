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
    public class HomeController : Controller
    {
        private static readonly FacebookClient client = new FacebookClient
        {
            ClientIdentifier = ConfigurationManager.AppSettings["facebookAppID"],
            ClientSecret = ConfigurationManager.AppSettings["facebookAppSecret"],
        };

        public void Index(string icode)
        {
            FacebookGraph fbUser;
            if (StateManager.IsAuthenticated(out fbUser))
            {
                var user = UsersManager.ConvertFacebookUser(fbUser);
                if (!UsersService.Exists(user.id))
                {
                    UsersService.Save(user);
                }

                if (! String.IsNullOrEmpty(icode))
                {
                    var site = SitesService.GetSiteByCode(icode);
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
            else
            {
                if (Facebook())
                {
                    Index(icode);
                }
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

        public bool Facebook()
        {
            IAuthorizationState authorization = client.ProcessUserAuthorization();
            if (authorization == null)
            {
                // Kick off authorization request
                client.RequestUserAuthorization();
            }
            else
            {
                var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString(authorization.AccessToken));
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var graph = FacebookGraph.Deserialize(responseStream);

                        StateManager.SetUser(graph);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
