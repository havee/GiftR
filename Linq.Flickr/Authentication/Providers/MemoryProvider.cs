using Linq.Flickr.Repository.Abstraction;
namespace Linq.Flickr.Authentication.Providers
{
    public class MemoryProvider : AuthenticaitonProvider
    {
        private readonly AuthInfo authenticationInformation;

        public MemoryProvider(IFlickrElement elementProxy, AuthInfo authenticationInformation)
            : base(elementProxy, authenticationInformation)
        {
            this.authenticationInformation = authenticationInformation;
        }

        public override AuthToken GetToken(string permission)
        {
            return authenticationInformation.AuthToken;
        }

        public override bool SaveToken(string permission)
        {
            return true;
        }

    }
}