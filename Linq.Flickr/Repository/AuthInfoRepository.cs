using Linq.Flickr.Authentication;
using Linq.Flickr.Proxies;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr.Repository
{
    public class AuthInfoRepository : IRepositoryFactory
    {
        public AuthInfoRepository()
        {
            this.elementProxy = new FlickrElementProxy();
        }

        public AuthInfoRepository(AuthInfo authenticationInformation) : this()
        {
            this.authenticationInformation = authenticationInformation;
        }

        public IAuthRepository CreateAuthRepository()
        {
            return new AuthRepository(elementProxy, authenticationInformation);
        }

        public ICommentRepository CreateCommentRepository()
        {
            return new CommentRepository(this.elementProxy, authenticationInformation);
        }

        public IPeopleRepository CreatePeopleRepository()
        {
            return new PeopleRepository(elementProxy, authenticationInformation);
        }

        public ITagRepository CreateTagRepository()
        {
            return new TagRepository(elementProxy, authenticationInformation, CreateAuthRepository());
        }

        public IPhotoRepository CreatePhotoRepository()
        {
            return new PhotoRepository(this.elementProxy, authenticationInformation);
        }

        private readonly AuthInfo authenticationInformation;
        private IFlickrElement elementProxy;
    }
}