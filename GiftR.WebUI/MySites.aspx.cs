using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GiftR.Services;
using GiftR.Common;
using GiftR.WebUI.Code;

namespace GiftR.WebUI
{
    public partial class MySites : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSites();
            }
        }

        private void BindSites()
        {
            // rptSites.DataSource = SitesService.GetSiteByUserId(StateManager.GetCurrentUser());
            rptSites.DataBind();
        }

        public string GetSiteUrl(string verification_code)
        {
            var url = new UriBuilder(PageFlowExtensions.GetDefaultPageUrl());
            url.Query += "icode=" + verification_code;

            return url.Uri.AbsoluteUri;
        }

        public void btnDelete_Command(object sender, CommandEventArgs e)
        {
            SitesService.Delete(Convert.ToInt32(e.CommandArgument));
            BindSites();
        }
    }
}