using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.Flickr.Attribute;

namespace Linq.Flickr.Repository.Abstraction
{
    public interface ITagRepository : IDisposable
    {
        // comment 
        [FlickrMethod("flickr.tags.getHotList")]
        IEnumerable<Tag> GetPopularTags(TagPeriod period, int count);
        [FlickrMethod("flickr.tags.getListPhoto")]
        IEnumerable<Tag> GetTagsForPhoto(string photoId);
        [FlickrMethod("flickr.photos.removeTag")]
        bool RemovTag(string tagId);
        [FlickrMethod("flickr.photos.addTags")]
        bool AddTags(string photoId, string tags);
    }
}
