using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Proxies;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr.Repository
{
    public class DefaultRepositoryFactory : IRepositoryFactory
    {
        private IFlickrElement elementProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRepositoryFactory"/> class.
        /// </summary>
        public DefaultRepositoryFactory()
        {
            this.elementProxy = new FlickrElementProxy(new WebRequestProxy());
        }

        public IAuthRepository CreateAuthRepository()
        {
            return new AuthRepository(elementProxy);
        }

        public ICommentRepository CreateCommentRepository()
        {
            return new CommentRepository(this.elementProxy);
        }

        public IPeopleRepository CreatePeopleRepository()
        {
            return new PeopleRepository(elementProxy);
        }

        public ITagRepository CreateTagRepository()
        {
            return new TagRepository(elementProxy);
        }

        public IPhotoRepository CreatePhotoRepository()
        {
            return new PhotoRepository(elementProxy);
        }
    }
}