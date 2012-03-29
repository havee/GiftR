namespace DotNetOpenAuth.ApplicationBlock {
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Globalization;
	using System.IO;
	using System.Net;
	using System.Web;
	using System.Xml;
	using System.Xml.Linq;
	using System.Xml.XPath;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OAuth;
	using DotNetOpenAuth.OAuth.ChannelElements;

	/// <summary>
	/// A consumer capable of communicating with Flickr.
	/// </summary>
	public static class FlickrConsumer {
		/// <summary>
		/// The description of Flickr's OAuth protocol URIs for use with actually reading/writing
		/// a user's private Flickr data.
		/// </summary>
		public static readonly ServiceProviderDescription ServiceDescription = new ServiceProviderDescription {
            RequestTokenEndpoint = new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/authorize", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
			TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
		};

		/// <summary>
		/// The description of Flickr's OAuth protocol URIs for use with their "Sign in with Flickr" feature.
		/// </summary>
		public static readonly ServiceProviderDescription SignInWithFlickrServiceDescription = new ServiceProviderDescription {
            RequestTokenEndpoint = new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/authorize", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
			TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
		};
		
		/// <summary>
		/// The consumer used for the Sign in to Flickr feature.
		/// </summary>
		private static WebConsumer signInConsumer;


        private static readonly MessageReceivingEndpoint VerifyCredentialsEndpoint = new MessageReceivingEndpoint("http://api.flickr.com/services/rest/?method=flickr.test.login", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);


		/// <summary>
		/// The lock acquired to initialize the <see cref="signInConsumer"/> field.
		/// </summary>
		private static object signInConsumerInitLock = new object();

		/// <summary>
		/// Initializes static members of the <see cref="FlickrConsumer"/> class.
		/// </summary>
		static FlickrConsumer() {
			
		}

		/// <summary>
		/// Gets a value indicating whether the Flickr consumer key and secret are set in the web.config file.
		/// </summary>
		public static bool IsFlickrConsumerConfigured {
			get {
				return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["flickrConsumerKey"]) &&
					!string.IsNullOrEmpty(ConfigurationManager.AppSettings["flickrConsumerSecret"]);
			}
		}

        public static string FlickrConsumerKey
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["flickrConsumerKey"]) ? "" : ConfigurationManager.AppSettings["flickrConsumerKey"];                    
            }
        }

        public static string FlickrConsumerSecret
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["flickrConsumerSecret"]) ? "" : ConfigurationManager.AppSettings["flickrConsumerSecret"];                    
            }
        }

		/// <summary>
		/// Gets the consumer to use for the Sign in to Flickr feature.
		/// </summary>
		/// <value>The twitter sign in.</value>
		private static WebConsumer FlickrSignIn {
			get {
				if (signInConsumer == null) {
					lock (signInConsumerInitLock) {
						if (signInConsumer == null) {
							signInConsumer = new WebConsumer(SignInWithFlickrServiceDescription, ShortTermUserSessionTokenManager);
						}
					}
				}

				return signInConsumer;
			}
		}

		public static InMemoryTokenManager ShortTermUserSessionTokenManager {
			get {
				var store = HttpContext.Current.Session;
				var tokenManager = (InMemoryTokenManager)store["FlickrShortTermUserSessionTokenManager"];
				if (tokenManager == null) {
                    string consumerKey = ConfigurationManager.AppSettings["flickrConsumerKey"];
                    string consumerSecret = ConfigurationManager.AppSettings["flickrConsumerSecret"];
					if (IsFlickrConsumerConfigured) {
						tokenManager = new InMemoryTokenManager(consumerKey, consumerSecret);
                        store["FlickrShortTermUserSessionTokenManager"] = tokenManager;
					} else {
                        throw new InvalidOperationException("No Flickr OAuth consumer key and secret could be found in web.config AppSettings.");
					}
				}

				return tokenManager;
			}
		}		

		public static XDocument VerifyCredentials(ConsumerBase flickr, string accessToken) {
			IncomingWebResponse response = flickr.PrepareAuthorizedRequestAndSend(VerifyCredentialsEndpoint, accessToken);
			return XDocument.Load(XmlReader.Create(response.GetResponseReader()));
		}

        public static XDocument GetAuthToken(ConsumerBase flickr, string accessToken) {
            IncomingWebResponse response = flickr.PrepareAuthorizedRequestAndSend(ServiceDescription.AccessTokenEndpoint, accessToken);
			return XDocument.Load(XmlReader.Create(response.GetResponseReader()));
		}
        

        public static string GetUsername(ConsumerBase flickr, string accessToken)
        {
            if (accessToken != null)
            {
                XDocument xml = VerifyCredentials(flickr, accessToken);
                XPathNavigator nav = xml.CreateNavigator();
                return nav.SelectSingleNode("/user/screen_name").Value;
            }

            return null;
        }

        public static string GetUserID(ConsumerBase flickr, string accessToken)
        {
            if (accessToken != null)
            {
                XDocument xml = VerifyCredentials(flickr, accessToken);
                XPathNavigator nav = xml.CreateNavigator();
                return nav.SelectSingleNode("/rsp/user").GetAttribute("id", "");
            }

            return null;
        }

		/// <summary>
		/// Prepares a redirect that will send the user to Flickr to sign in.
		/// </summary>
		/// <param name="forceNewLogin">if set to <c>true</c> the user will be required to re-enter their Flickr credentials even if already logged in to Flickr.</param>
		/// <returns>The redirect message.</returns>
		/// <remarks>
		/// Call <see cref="OutgoingWebResponse.Send"/> or
		/// <c>return StartSignInWithFlickr().<see cref="MessagingUtilities.AsActionResult">AsActionResult()</see></c>
		/// to actually perform the redirect.
		/// </remarks>
		public static OutgoingWebResponse StartSignInWithFlickr() {
			var redirectParameters = new Dictionary<string, string>();

			Uri callback = MessagingUtilities.GetRequestUrlFromContext().StripQueryArgumentsWithPrefix("oauth_");
			var request = FlickrSignIn.PrepareRequestUserAuthorization(callback, null, redirectParameters);
            return FlickrSignIn.Channel.PrepareResponse(request);
		}

        public static void SignInWithFlickr()
        {
            FlickrSignIn.Channel.Send(FlickrSignIn.PrepareRequestUserAuthorization());
        }
		/// <summary>
		/// Checks the incoming web request to see if it carries a Flickr authentication response,
		/// and provides the user's Flickr screen name and unique id if available.
		/// </summary>
		/// <param name="screenName">The user's Flickr screen name.</param>
		/// <param name="userId">The user's Flickr unique user ID.</param>
		/// <returns>
		/// A value indicating whether Flickr authentication was successful;
		/// otherwise <c>false</c> to indicate that no Flickr response was present.
		/// </returns>
		public static bool TryFinishSignInWithFlickr(out string screenName, out string userId, out string access_token) {
			screenName = null;
			userId = "";
            access_token = "";

            var response = FlickrSignIn.ProcessUserAuthorization();
			if (response == null) {
				return false;
			}

            access_token = response.AccessToken;
			screenName = response.ExtraData["fullname"];
            userId = response.ExtraData["user_nsid"];

			// If we were going to make this LOOK like OpenID even though it isn't,
			// this seems like a reasonable, secure claimed id to allow the user to assume.
			OpenId.Identifier fake_claimed_id = string.Format(CultureInfo.InvariantCulture, "http://flickr.com/{0}#{1}", screenName, userId);

			return true;
		}
	}
}
