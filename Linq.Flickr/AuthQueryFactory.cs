using Linq.Flickr.Authentication;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr
{
    public class AuthQueryFactory : IQueryFactory
    {
        public AuthQueryFactory(IFlickrElement elementProxy, AuthInfo authenticationInformation)
        {
            this.elementProxy = elementProxy;
            this.authenticationInformation = authenticationInformation;
        }

        public TagCollection CreateTagQuery()
        {
            return new TagCollection(authenticationInformation);
        }

        public PeopleCollection CreatePeopleQuery()
        {
            return new PeopleCollection(authenticationInformation);
        }

        public PhotoCollection CreatePhotoQuery()
        {
            return new PhotoCollection(elementProxy, authenticationInformation);
        }

        private readonly AuthInfo authenticationInformation;
        private IFlickrElement elementProxy;
    }
}