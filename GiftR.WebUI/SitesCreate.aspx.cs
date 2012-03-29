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

namespace GiftR.WebUI
{
    public partial class SitesCreate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            FacebookGraph fbUser;
            if (StateManager.IsAuthenticated(out fbUser))
            {
                var usersRepo = new UsersRepository();
                var user = usersRepo.GetUserByExternalId(fbUser.Id);
                if (user != null)
                {
                    user.email = txtEmail.Text;
                    usersRepo.SaveChanges();

                    SitesRepository sitesRepo = new SitesRepository();
                    var code = Guid.NewGuid().ToString();
                    var site = new Sites()
                    {
                        flickr_username = txtFlickrUsername.Text,
                        title = txtTitle.Text,
                        site_type = Convert.ToInt32(txtType.Text),
                        verificacion_code = code
                    };
                    sitesRepo.Save(site);

                    Response.Write(HttpExtensions.GetDefaultPageUrl().OriginalString + "?icode=" + code);
                }                
            }
        }
    }
}