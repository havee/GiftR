using System;
using System.Text;
using System.Web;
using Linq.Flickr.Configuration;
using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Repository;
using DotNetOpenAuth.ApplicationBlock;

namespace Linq.Flickr.Authentication.Providers
{
    public class WebProvider : AuthenticaitonProvider
    {
        public WebProvider(IFlickrElement elementProxy)
            : base(elementProxy)
        {
            this.elementProxy = elementProxy;
            flickrSettingsProvider = new ConfigurationFileFlickrSettingsProvider();
        }

        public override bool SaveToken(string permission)
        {
            IAuthRepository authRepository = new AuthRepository(elementProxy);
            try
            {
                bool authenticate = false;

                string frob = CreateWebFrobIfNecessary(out authenticate);

                if (authenticate)
                {
                    /// initiate the authenticaiton process.
                    HttpContext.Current.Response.Redirect(GetAuthenticationUrl(permission));
                }

                AuthToken token = authRepository.GetTokenFromFrob(frob);

                if (token != null)
                {
                    OnAuthenticationComplete(token);
                }

                return true;
            }
            catch(Exception ex)
            {
               /// failed
               System.Diagnostics.Debug.WriteLine(ex.Message);
               return false;
            }
        }

        public override void OnAuthenticationComplete(AuthToken token)
        {
            string xml = XmlToObject<AuthToken>.Serialize(token);
            /// create a cookie out of it.
            var authCookie = new HttpCookie("token", HttpUtility.UrlEncode(xml));
            /// set exipration.
            authCookie.Expires = DateTime.Now.AddDays(30);
            /// put it to response.
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        public override AuthToken GetToken(string permission)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["token"] != null)
                {
                    if (HttpContext.Current.Request.Cookies != null)
                    {
                        var authToken = FlickrConsumer.GetAuthToken(new DotNetOpenAuth.OAuth.WebConsumer(FlickrConsumer.ServiceDescription, FlickrConsumer.ShortTermUserSessionTokenManager)
                            ,HttpContext.Current.Request.Cookies["token"].Value);

                        string xml = HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["token"].Value);
                        if (!string.IsNullOrEmpty(xml))
                        {
                            return XmlToObject<AuthToken>.Deserialize(xml);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                /// must do a authentication.
                return null;
            }
            return null;
        }


        public override void OnClearToken(AuthToken token)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["token"];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        private string CreateWebFrobIfNecessary(out bool authenticated)
        {
            IRepositoryBase repositoryBase = new CommonRepository(elementProxy);
            // if it is a redirect by flickr then take the frob from url.
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["frob"]))
            {
                authenticated = false;
                return HttpContext.Current.Request["frob"];
            }
            else
            {
                authenticated = true;
                return repositoryBase.GetFrob();
            }
        }

        private string GetAuthenticationUrl(string permission)
        {
            string apiKey = flickrSettingsProvider.GetCurrentFlickrSettings().ApiKey;
            string sig = new CommonRepository(elementProxy).GetSignature(string.Empty, false, "perms", permission);

            StringBuilder builder = new StringBuilder(Helper.AUTH_URL + "?api_key=" + apiKey);

            builder.Append("&perms=" + permission);
            builder.Append("&api_sig=" + sig);

            return builder.ToString();

        }

        private IFlickrSettingsProvider flickrSettingsProvider;
        private IFlickrElement elementProxy;
    }
}
