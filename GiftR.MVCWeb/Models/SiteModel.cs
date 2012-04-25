using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GiftR.Model;

namespace GiftR.MVCWeb.Models
{
    public class SiteModel
    {
        public string Email { get; set; }

        public int SiteType { get; set; }

        public string Title { get; set; }

        public string FlickrUserName { get; set; }

        public string Verification_Code { get; set; }

        public int Id { get; set; }

        public string Url { get; set; }
    }
}