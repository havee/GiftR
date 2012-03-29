using System;
using Linq.Flickr.Attribute;

namespace Linq.Flickr.Repository.Abstraction
{
    public interface IAuthRepository : IDisposable
    {
        [FlickrMethod("flickr.auth.getToken")]
        AuthToken GetTokenFromFrob(string frob);
        [FlickrMethod("flickr.auth.checkToken")]
        AuthToken CheckToken(string token);
        /// <summary>
        /// Tries to authenticate with provided permission.
        /// </summary>
        /// <param name="validate"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        AuthToken Authenticate(bool validate, Permission permission);
        bool IsAuthenticated();
        AuthToken CreateAuthTokenIfNecessary(string permission, bool validate);
        /// <summary>
        /// Tries a authentication for a given permission.
        /// </summary>
        /// <param name="permission"></param>
        string Authenticate(Permission permission);
        /// <summary>
        /// Tries a authentication for given permission, if there is a existing 
        /// token it takes it. Otherwise, tries for a new one from flickr.
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="validate"></param>
        /// <returns>token Id</returns>
        string Authenticate(string permission, bool validate);
        /// <summary>
        /// Clears the auth token from local store.
        /// </summary>
        void ClearToken();
    }
}
