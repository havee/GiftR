using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Linq.Flickr;
using LinqExtender;

using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth;
using System.Configuration;
using Linq.Flickr.Authentication;
using Linq.Flickr.Configuration;
using Linq.Flickr.Repository;
using Linq.Flickr.Authentication.Providers;
using Linq.Flickr.Proxies;
using Linq.Flickr.Repository.Abstraction;
using System.Text;
using DotNetOpenAuth.ApplicationBlock.Facebook;

using GiftR.Common;
using GiftR.Repository;
using GiftR.WebUI.Code;
using GiftR.Model;
using GiftR.Services;

namespace GiftR.WebUI
{
    public partial class Default : Page
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FacebookGraph fbUser;
                if (StateManager.IsAuthenticated(out fbUser))
                {
                    var user = UsersManager.ConvertFacebookUser(fbUser);
                    if (! UsersService.Exists(user.id))
                    {
                        UsersService.Save(user);
                    }

                    GetSite();
                }
                else
                {
                    Response.RedirectWithQueryString("Facebook.aspx");
                }                
            }
        }

        private void GetSite()
        {
            if (Request.QueryString["icode"] != null)
            {
                var site = SitesService.GetSiteByCode(Request.QueryString["icode"]);
                if (site != null)
                {
                    StateManager.AddSite(site);
                    Response.RedirectWithQueryString(site.SitesTypes.default_page);
                }
            }
            else
            {
                Response.RedirectWithQueryString(PageFlowExtensions.GetCreateSitePageUrl().AbsoluteUri);
            }
        }

        void Photos_OnError(ProviderException providerException)
        {
            throw new NotImplementedException();
        }        


    }
}