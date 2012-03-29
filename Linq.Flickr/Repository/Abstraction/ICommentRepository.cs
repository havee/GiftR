using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.Flickr.Attribute;

namespace Linq.Flickr.Repository.Abstraction
{
    public interface ICommentRepository : IDisposable
    {
        // comment 
        [FlickrMethod("flickr.photos.comments.getList")]
        IEnumerable<Comment> GetComments(string photoId);
        [FlickrMethod("flickr.photos.comments.addComment")]
        string AddComment(string photoId, string text);
        [FlickrMethod("flickr.photos.comments.deleteComment")]
        bool DeleteComment(string commentId);
        [FlickrMethod("flickr.photos.comments.editComment")]
        bool EditComment(string photoId, string text);
        
    }
}
