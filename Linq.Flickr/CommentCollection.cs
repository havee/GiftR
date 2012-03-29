using System;
using System.Collections.Generic;
using System.Linq;
using LinqExtender;
using Linq.Flickr.Repository;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr
{
    public class CommentCollection : Query<Comment>
    {
        public CommentCollection(IFlickrElement elementProxy)
        {
            this.elementProxy = elementProxy;
        }

        protected override bool AddItem()
        {
            string photoId = (string)Bucket.Instance.For.Item(CommentColumns.PhotoId).Value;
            string text = (string)Bucket.Instance.For.Item(CommentColumns.Text).Value;

            if (string.IsNullOrEmpty(photoId))
            {
                throw new Exception("Must have valid photoId");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Must have some text for the comment");
            }

            using (ICommentRepository commentRepositoryRepo = new CommentRepository(elementProxy))
            {
                string commentId = commentRepositoryRepo.AddComment(photoId, text);
                // set the id.
                Bucket.Instance.For.Item(CommentColumns.Id).Value = commentId;

                return (string.IsNullOrEmpty(commentId) == false);
            }
        }

        protected override bool UpdateItem()
        {
            string commentId = (string)Bucket.Instance.For.Item(CommentColumns.Id).Value;
            string text = (string)Bucket.Instance.For.Item(CommentColumns.Text).Value;

            if (string.IsNullOrEmpty(commentId))
                throw new Exception("Invalid comment Id");

            if (string.IsNullOrEmpty(text))
                throw new Exception("Blank comment is not allowed");

            using (ICommentRepository commentRepositoryRepo = new CommentRepository(elementProxy))
            {
                return commentRepositoryRepo.EditComment(commentId, text); 
            }
        }

        protected override bool RemoveItem()
        {
            string commentId = (string)Bucket.Instance.For.Item(CommentColumns.Id).Value;

            if (string.IsNullOrEmpty(commentId))
            {
                throw new Exception("Must provide a comment_id");
            }
            using (ICommentRepository commentRepositoryRepo = new CommentRepository(elementProxy))
            {
                return commentRepositoryRepo.DeleteComment(commentId);
            }
        }

        private static class CommentColumns
        {
            public const string Id = "Id";
            public const string PhotoId = "PhotoId";
            public const string Text = "Text";
        }

        protected override void Process(LinqExtender.Interface.IModify<Comment> items)
        {
            using (ICommentRepository commentRepositoryRepo = new CommentRepository(elementProxy))
            {
                string photoId = (string) Bucket.Instance.For.Item(CommentColumns.PhotoId).Value;
                string commentId = (string)Bucket.Instance.For.Item(CommentColumns.Id).Value;

                if (string.IsNullOrEmpty(photoId))
                {
                    throw new Exception("Must have a valid photoId");
                }

                int index = Bucket.Instance.Entity.ItemsToSkipFromStart;
                int itemsToTake = int.MaxValue;

                if (Bucket.Instance.Entity.ItemsToFetch != null)
                {
                    itemsToTake = Bucket.Instance.Entity.ItemsToFetch.Value;
                }

                // get comments
                IEnumerable<Comment> comments = commentRepositoryRepo.GetComments(photoId);
                // filter 
                if (!string.IsNullOrEmpty(commentId))
                {
                    var query = (from comment in comments
                                where comment.Id == commentId
                                select comment).Skip(index).Take(itemsToTake);
                    comments = query;
                }
                else
                {
                    var query = (from comment in comments
                                 select comment).Skip(index).Take(itemsToTake);
                    comments = query;
                }
                items.AddRange(comments, true);
            }
        }

        private IFlickrElement elementProxy;
    }
}
