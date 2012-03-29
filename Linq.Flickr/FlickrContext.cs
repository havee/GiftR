using System;
using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Repository;
using Linq.Flickr.Authentication;
using Linq.Flickr.Proxies;

namespace Linq.Flickr
{
    /// <summary>
    /// Entry point for querying flickr photos.
    /// </summary>
    public class FlickrContext 
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="FlickrContext"/> class.
        /// </summary>
        public FlickrContext(IFlickrElement elementProxy)
        {
            this.elementProxy = elementProxy;
            this.queryFactory = new DefaultQueryFactory(elementProxy);
        }

        /// <summary>
        /// Initalizes a new instance of the <see cref="FlickrContext"/> class.
        /// </summary>
        public FlickrContext(AuthInfo authInfo) : this(new FlickrElementProxy(new WebRequestProxy()))
        {
            this.authenticationInformation = authInfo;
            this.queryFactory = new AuthQueryFactory(elementProxy, authInfo);
        }

        /// <summary>
        /// Initalizes a new instance of the <see cref="FlickrContext"/> class.
        /// </summary>
        public FlickrContext(): this(new FlickrElementProxy(new WebRequestProxy()))
        {
        }

        public PhotoCollection Photos
        {
            get
            {
                if (photos == null)
                    photos = queryFactory.CreatePhotoQuery();
                return photos;
            }
        }

        public TagCollection Tags
        {
            get
            {
                if (tags == null)
                {
                    tags = queryFactory.CreateTagQuery();
                }

                return tags;
            }
        }

        public PeopleCollection Peoples
        {
            get
            {
                if (peoples == null)
                {
                    peoples = queryFactory.CreatePeopleQuery();
                }
                return peoples;
            }
        }
        /// <summary>
        /// check if the user is already authicated for making authenticated calls.
        /// </summary>
        /// <returns>returns true/false</returns>
        public bool IsAuthenticated()
        {
            using (IAuthRepository authRepository = CreateNewAuthRepository())
            {
                return authRepository.IsAuthenticated();
            }
        }

        /// <summary>
        /// does a manual authentication.
        /// </summary>
        public AuthToken Authenticate()
        {
            using (IAuthRepository authRepository = CreateNewAuthRepository())
            {
                return authRepository.Authenticate(true, Permission.Delete);
            }
        }

        /// <summary>
        /// removes the token from cache or cookie.
        /// </summary>
        /// <returns></returns>
        public bool ClearToken()
        {
            bool result = true;

            try
            {
                IAuthRepository repository = CreateNewAuthRepository();
                repository.ClearToken();
            }
            catch
            {
                result = false;
            }

            return result;
        }


        public void SubmitChanges()
        {
            photos.SubmitChanges();
            // sync changed comments, if any.
            photos.Comments.SubmitChanges();
        }

        private IAuthRepository CreateNewAuthRepository()
        {
            IAuthRepository authRepository;
            if (authenticationInformation != null)
                authRepository = CreateAuthRepositoryWithProvidedAuthenticationInformation();
            else
                authRepository = new AuthRepository(elementProxy);
            return authRepository;
        }

        private AuthRepository CreateAuthRepositoryWithProvidedAuthenticationInformation()
        {
            return new AuthRepository(elementProxy, authenticationInformation);
        }


        private PhotoCollection photos;
        private TagCollection tags;
        private PeopleCollection peoples;

        private readonly AuthInfo authenticationInformation;
        private readonly IQueryFactory queryFactory;
        private readonly IFlickrElement elementProxy;
    }
}
