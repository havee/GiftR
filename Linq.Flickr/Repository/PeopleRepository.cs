using System;
using System.Collections.Generic;
using System.Linq;
using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Authentication;

namespace Linq.Flickr.Repository
{
    public class PeopleRepository : CommonRepository, IPeopleRepository
    {
        public PeopleRepository(IFlickrElement elementProxy)
            : base(elementProxy, typeof(IPeopleRepository))
        {
            this.elementProxy = elementProxy;
            authRepo = new AuthRepository(elementProxy);
        }

        public PeopleRepository(IFlickrElement elementProxy, AuthInfo authenticationInformation)
            : base(elementProxy, authenticationInformation, typeof(IPeopleRepository))
        {
            this.elementProxy = elementProxy;
            authRepo = new AuthRepository(elementProxy, authenticationInformation);
        }

        #region IPeopleRepository Members

        People IPeopleRepository.GetInfo(string userId)
        {
            string method = Helper.GetExternalMethodName();
            string sig = GetSignature(method, true, "user_id", userId);
            string requestUrl = BuildUrl(method, "user_id", userId, "api_sig", sig);
            return GetPeople(requestUrl).Single();
        }

        People IPeopleRepository.GetByUsername(string username)
        {
            string nsId = string.Empty;

            using (IPhotoRepository photoRepository = new PhotoRepository(elementProxy))
            {
                nsId = photoRepository.GetNsidByUsername(username);

                if (!string.IsNullOrEmpty(nsId))
                {
                    return (this as IPeopleRepository).GetInfo(nsId);
                }
                else
                {
                    throw new Exception("Invalid user Id");
                }
            }
        }

        AuthToken IPeopleRepository.GetAuthenticatedToken()
        {
            return  authRepo.CreateAuthTokenIfNecessary(Permission.Delete.ToString(), false);
        }

        #endregion

        private IEnumerable<People> GetPeople(string requestUrl)
        {
            CollectionBuilder<People> rest = new CollectionBuilder<People>();
            return rest.ToCollection(requestUrl, null);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        private IAuthRepository authRepo;
        private IFlickrElement elementProxy;

    }
}
