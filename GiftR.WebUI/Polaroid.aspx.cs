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
    public partial class Polaroid : Page
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
                if (!SessionManager.IsAuthenticated(out fbUser))
                {
                    Response.RedirectWithQueryString("Facebook.aspx");
                }

                loggedInName.Text = "Bienvenido, " + fbUser.Name;

                FlickrContext context = new FlickrContext();
                context.Photos.OnError += new Query<Photo>.ErrorHandler(Photos_OnError);

                var bigs = (from ph in context.Photos
                            where ph.User == "havee1983" 
                            && ph.PhotoSize == PhotoSize.Default
                            select ph).ToList();

                var thumbs = (from ph in context.Photos
                           where ph.User == "havee1983"
                           && ph.PhotoSize == PhotoSize.Thumbnail
                           select ph).ToList();

                var query = from b in bigs
                            join t in thumbs on b.Id equals t.Id
                            select new Image() { Src = t.Url, Alt = b.Url, Id = t.Id, Title = t.Title };
                
                rptAlbum.DataSource = query.ToList();
                rptAlbum.DataBind();
            }
        }

        void Photos_OnError(ProviderException providerException)
        {
            throw new NotImplementedException();
        }    
    }
}