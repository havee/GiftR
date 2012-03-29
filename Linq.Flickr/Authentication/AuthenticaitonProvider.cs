using Linq.Flickr.Repository.Abstraction;
using Linq.Flickr.Repository;

namespace Linq.Flickr.Authentication
{
    public abstract  class  AuthenticaitonProvider : CommonRepository
    {
        public AuthenticaitonProvider(IFlickrElement elementProxy) : base(elementProxy)
        {
        }

        public AuthenticaitonProvider(IFlickrElement elementProxy, AuthInfo authenticationInformation)
            : base (elementProxy, authenticationInformation)
        {
        }

        /// <summary>
        /// Saves a token to the disc,if not already saved and initiates flickr 
        /// authentication call.
        /// </summary>
        /// <param name="permission">Read|Write|Delete</param>
        public virtual bool SaveToken(string permission)
        {
            return false;
        }
        /// <summary>
        /// Gets a token from the disc.
        /// </summary>
        /// <param name="permission">Read|Write|Delete</param>
        /// <returns></returns>
        public virtual AuthToken GetToken(string permission)
        {
            return null;
        }

        /// <summary>
        /// This is where the authentication token is saved to disc,
        /// ovverride it to define your own save logic.
        /// </summary>
        /// <param name="token"></param>
        public virtual void OnAuthenticationComplete(AuthToken token)
        {
        
            
        }
        /// <summary>
        /// This is class when user explicitly specifies a token removal.
        /// </summary>
        /// <param name="permission"></param>
        public virtual void ClearToken(string permission)
        {
            OnClearToken(GetToken(permission));   
        }

        /// <summary>
        /// Invoked when a token is being cleared, add your own logic to clear a token.
        /// </summary>
        /// <param name="token"></param>
        public virtual void OnClearToken(AuthToken token)
        {
            
        }
    }
}