using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetOpenAuth.ApplicationBlock.Facebook;

namespace GiftR.Common
{
    public class SessionManager
    {
        private const string _facebook_session_key = "__facebook_session_key";

        public static void SetUser(FacebookGraph user)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session.Add(_facebook_session_key, user);
            }
        }

        public static FacebookGraph GetCurrentUser()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session[_facebook_session_key] != null)
                {
                    return (FacebookGraph)HttpContext.Current.Session[_facebook_session_key];
                }
            }

            return null;
        }

        public static bool IsAuthenticated(out FacebookGraph user)
        {
            user = new FacebookGraph();
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session[_facebook_session_key] != null)
                {
                    user = (FacebookGraph)HttpContext.Current.Session[_facebook_session_key];

                    return true;
                }
            }

            return false;
        }

    }
}
