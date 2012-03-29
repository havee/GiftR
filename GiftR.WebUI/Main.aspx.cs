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

namespace GiftR.WebUI
{
    public partial class Main : Page
    {
        public class Image
        {
            public string Src { get; set; }

            public string Alt { get; set; }

            public string Title { get; set; }

            public string Id { get; set; }
        }           

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FacebookGraph fbUser;
                if (SessionManager.IsAuthenticated(out fbUser))
                {
                    UsersRepository usersRepo = new UsersRepository();
                    var user = UsersManager.ConvertFacebookUser(fbUser);
                    if (! usersRepo.Exists(user))
                    {
                        usersRepo.SaveUser(user);
                    }

                    Response.RedirectWithQueryString("Polaroid.aspx");
                }
                else
                {
                    Response.RedirectWithQueryString("Facebook.aspx");
                }                
            }
        }

        void Photos_OnError(ProviderException providerException)
        {
            throw new NotImplementedException();
        }        

        protected void signInButton_Click(object sender, ImageClickEventArgs e)
        {      
        }
    }
}