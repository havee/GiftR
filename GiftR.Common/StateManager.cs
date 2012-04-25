using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetOpenAuth.ApplicationBlock.Facebook;
using GiftR.Model;
using System.Web.Security;

namespace GiftR.Common
{
    public class StateManager
    {
        private const string _facebook_session_key = "__facebook_session__key";
        private const string _modeluser_session_key = "__modeluser_session__key";
        private const string _application_sites_key = "__application_sites__key";

        private static HttpContext Context
        {
            get
            {
                if (HttpContext.Current == null) throw new Exception("The current context is null");

                return HttpContext.Current;
            }
        }

        public static string GetCurrentUser()
        {
            var identity = (FormsIdentity)HttpContext.Current.User.Identity;
            var userdata = identity.Ticket.UserData;

            return userdata;
        }

        public static bool IsAuthenticated(out FacebookGraph user)
        {
            user = new FacebookGraph();
            if (Context.Session[_facebook_session_key] != null)
            {
                user = (FacebookGraph)Context.Session[_facebook_session_key];

                return true;
            }            

            return false;
        }

        protected static Dictionary<string, Sites> Sites
        {
            get
            {
                if (Context.Application[_application_sites_key] == null)
                {
                    Context.Application[_application_sites_key] = new Dictionary<string, Sites>();
                }

                return (Dictionary<string, Sites>)Context.Application[_application_sites_key];
            }
            set
            {
                Context.Application[_application_sites_key] = value;
            }
        }

        public static void AddSite(Sites site)
        {
            var sites = Sites;
            if (sites.ContainsKey(site.verification_code))
            {
                sites.Remove(site.verification_code);
            }

            sites.Add(site.verification_code, site);
        }

        public static Sites GetSite(string code)
        {
            var sites = Sites;
            if (sites.ContainsKey(code))
            {
                return Sites[code];
            }

            return null;
        }

    }
}
