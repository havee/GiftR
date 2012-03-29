using System;
using System.Collections.Generic;
using Linq.Flickr.Authentication;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr.Repository
{
    public class CommentRepository : CommonRepository, ICommentRepository
    {
        public CommentRepository(IFlickrElement elementProxy) : base(elementProxy, typeof(ICommentRepository))
        {
            this.elementProxy = elementProxy;
            authRepo = new AuthRepository(elementProxy);
        }

        public CommentRepository(IFlickrElement elementProxy, AuthInfo authenticationInformation)
            : base (elementProxy, authenticationInformation, typeof(ICommentRepository))
        {
            this.elementProxy = elementProxy;
            authRepo = new AuthRepository(elementProxy, authenticationInformation);
        }

        private IEnumerable<Comment> GetComments(string requestUrl)
        {
            CollectionBuilder<Comment> builder = new CollectionBuilder<Comment>("comments");
            return builder.ToCollection(requestUrl, null);
        }

        IEnumerable<Comment> ICommentRepository.GetComments(string photoId)
        {
            string method = Helper.GetExternalMethodName();
            AuthToken token = authRepo.Authenticate(false, Permission.Delete);

            string authenitcatedToken = string.Empty;

            if (token != null)
            {
                authenitcatedToken = token.Id;
            }
            string sig = GetSignature(method, true, "photo_id", photoId, "auth_token", authenitcatedToken);
            string requestUrl = BuildUrl(method, "photo_id", photoId, "api_sig", sig, "auth_token", authenitcatedToken);
            return GetComments(requestUrl);
        }
       
        string ICommentRepository.AddComment(string photoId, string text)
        {
            string authenitcatedToken =  authRepo.Authenticate(Permission.Write);

            string method = Helper.GetExternalMethodName();

            string sig = GetSignature(method, true, "photo_id", photoId, "auth_token", authenitcatedToken, "comment_text", text);
            string requestUrl = BuildUrl(method, "photo_id", photoId, "comment_text", text, "auth_token", authenitcatedToken, "api_sig", sig);

            var element = elementProxy.SendPostRequest(requestUrl);
            return element.Element("comment").Attribute("id").Value ?? string.Empty;
        }

        bool ICommentRepository.DeleteComment(string commentId)
        {
            string authenitcatedToken = authRepo.Authenticate(Permission.Delete);
            string method = Helper.GetExternalMethodName();

            string sig = GetSignature(method, true, "comment_id", commentId, "auth_token", authenitcatedToken);
            string requestUrl = BuildUrl(method, "comment_id", commentId, "auth_token", authenitcatedToken, "api_sig", sig);

            try
            {
                elementProxy.SendPostRequest(requestUrl);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool ICommentRepository.EditComment(string commentId, string text)
        {
            string method = Helper.GetExternalMethodName();
            string authenitcatedToken = authRepo.Authenticate(Permission.Delete);

            string sig = GetSignature(method, true, "comment_id", commentId, "comment_text", text, "auth_token", authenitcatedToken);
            string requestUrl = BuildUrl(method, "comment_id", commentId, "comment_text", text, "auth_token", authenitcatedToken, "api_sig", sig);

            try
            {
                elementProxy.SendPostRequest(requestUrl);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void IDisposable.Dispose()
        {

        }

        private IAuthRepository authRepo;
        private IFlickrElement elementProxy;
    }
}
