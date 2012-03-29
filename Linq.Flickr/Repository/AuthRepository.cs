using System;
using System.Xml;
using Linq.Flickr.Authentication;
using Linq.Flickr.Configuration;
using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Authentication.Providers;

namespace Linq.Flickr.Repository
{
    /// <summary>
    /// Authentication respository.
    /// </summary>
    public class AuthRepository : CommonRepository, IAuthRepository
    {
        public AuthRepository(IFlickrElement elementProxy) : base(elementProxy, typeof(IAuthRepository))
        {
            this.elementProxy = elementProxy;
        }

        public AuthRepository(IFlickrElement elementProxy, AuthInfo authenticationInformation)
            : base(elementProxy, authenticationInformation)
        {
            this.elementProxy = elementProxy;
            this.authenticationInformation = authenticationInformation;
        }

        public AuthToken Authenticate(bool validate, Permission permission)
        {
           return (this as IAuthRepository).CreateAuthTokenIfNecessary(permission.ToString().ToLower(), validate);
        }

        public AuthToken CreateAuthTokenIfNecessary(string permission, bool validate)
        {

            AuthenticaitonProvider authenticaitonProvider = GetDefaultAuthenticationProvider();

            permission = permission.ToLower();

            AuthToken token = authenticaitonProvider.GetToken(permission);

            if (token == null && validate)
            {
                authenticaitonProvider.SaveToken(permission);
            }

            return authenticaitonProvider.GetToken(permission);
        }

        public string Authenticate(string permission, bool validate)
        {
            AuthToken token = (this as IAuthRepository).CreateAuthTokenIfNecessary(permission, validate);
            if (token != null)
            {
                return token.Id;
            }
            return string.Empty;
        }

        public string Authenticate(Permission permission)
        {
            return (this as IAuthRepository).CreateAuthTokenIfNecessary(permission.ToString(), true).Id;
        }

        bool IAuthRepository.IsAuthenticated()
        {
            AuthenticaitonProvider authProvider = GetDefaultAuthenticationProvider();
            return authProvider.GetToken(Permission.Delete.ToString().ToLower()) != null;
        }

        AuthToken IAuthRepository.GetTokenFromFrob(string frob)
        {
            string method = Helper.GetExternalMethodName();

            string sig = GetSignature(method, true, "frob", frob);
            string requestUrl = BuildUrl(method, "api_sig", sig, "frob", frob);

            try
            {
                XmlElement tokenElement = elementProxy.GetResponseElement(requestUrl);
                return GetAToken(tokenElement);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        AuthToken IAuthRepository.CheckToken(string token)
        {
            string method = Helper.GetExternalMethodName();
            return ValidateToken(method, token);
        }

        private AuthenticaitonProvider GetDefaultAuthenticationProvider()
        {
            if (AuthenticationInformationWasProvidedManually())
                return new MemoryProvider(elementProxy, authenticationInformation);

            return GetTheDefaultProviderFromCurrentFlickrSettings();
        }

        void IAuthRepository.ClearToken()
        {
            AuthenticaitonProvider authenticaitonProvider = GetDefaultAuthenticationProvider();
            authenticaitonProvider.ClearToken(Permission.Delete.ToString().ToLower());
        }

        public void Dispose()
        {
            
        }

        private AuthenticaitonProvider GetTheDefaultProviderFromCurrentFlickrSettings()
        {
            var providerElement = Provider.GetCurrentFlickrSettings().DefaultProvider;
            return (AuthenticaitonProvider)Activator.CreateInstance(Type.GetType(providerElement.Type), new object[] {  elementProxy } );
        }

        private bool AuthenticationInformationWasProvidedManually()
        {
            return authenticationInformation != null;
        }

        private AuthInfo authenticationInformation;
        private IFlickrElement elementProxy;
    }
}
