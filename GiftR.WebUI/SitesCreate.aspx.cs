using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GiftR.Common;
using DotNetOpenAuth.ApplicationBlock.Facebook;
using GiftR.Repository;
using GiftR.Model;
using GiftR.WebUI.Code;
using DotNetOpenAuth.ApplicationBlock;
using System.Configuration;
using DotNetOpenAuth.OAuth;
using GiftR.Services;

namespace GiftR.WebUI
{
    public partial class SitesCreate : System.Web.UI.Page
    {
        private string AccessToken
        {
            get { return (string)Session["GoogleAccessToken"]; }
            set { Session["GoogleAccessToken"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            FacebookGraph fbUser;
            if (StateManager.IsAuthenticated(out fbUser))
            {
                var user = UsersService.GetUserByExternalId(fbUser.Id);
                if (user != null)
                {
                    var code = Guid.NewGuid().ToString();
                    var site = new Sites()
                    {
                        flickr_username = txtFlickrUsername.Text,
                        title = txtTitle.Text,
                        site_type = Convert.ToInt32(txtType.Text),
                        verificacion_code = code
                    };
                    SitesService.Save(user.id, txtEmail.Text, site);

                    this.pnlForm.Visible = false;
                    this.pnlMsg.Visible = true;
                    this.lblMsg.Text = "El sitio se ha creado satisfactoriamente, accedé ya al mismo:";

                    var shortUrl = GoogleConsumer.ShortenUrl(PageFlowExtensions.GetDefaultPageUrl().OriginalString + "?icode=" + code);

                    this.hlLink.NavigateUrl = shortUrl;
                    this.hlLink.Text = shortUrl;
                }            
            }
        }

        private InMemoryTokenManager TokenManager
        {
            get
            {
                var tokenManager = (InMemoryTokenManager)Application["GoogleTokenManager"];
                if (tokenManager == null)
                {
                    string consumerKey = ConfigurationManager.AppSettings["googleConsumerKey"];
                    string consumerSecret = ConfigurationManager.AppSettings["googleConsumerSecret"];
                    if (!string.IsNullOrEmpty(consumerKey))
                    {
                        tokenManager = new InMemoryTokenManager(consumerKey, consumerSecret);
                        Application["GoogleTokenManager"] = tokenManager;
                    }
                }

                return tokenManager;
            }
        }
    }
}