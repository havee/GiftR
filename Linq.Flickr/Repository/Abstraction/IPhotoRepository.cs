using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Linq.Flickr.Attribute;
using Linq.Flickr.Repository;

namespace Linq.Flickr.Repository.Abstraction
{
    public interface IPhotoRepository : IDisposable
    {
        [FlickrMethod("flickr.photos.getInfo")]
        Photo GetPhotoDetail(string id, PhotoSize size);
        /// <summary>
        /// Searchs the photos for supplied name value settings.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="pageLen">Number of items on each page</param>
        /// <param name="photoSize"></param>
        /// <param name="token">Authentication token, if logged in</param>
        /// <param name="args">Search criteria</param>
        /// <returns>list of photos</returns>
        [FlickrMethod("flickr.photos.search")]
        IEnumerable<Photo> Search(int index, int pageLen, PhotoSize photoSize, string token, params object[] args);
        [FlickrMethod("flickr.interestingness.getList")]
        IList<Photo> GetMostInteresting(int index, int itemsPerPage, PhotoSize photoSize);
        [FlickrMethod("flickr.photos.delete")]
        bool Delete(string photoId);
        [FlickrMethod("flickr.photos.getSizes")]
        string GetSizedPhotoUrl(string id, PhotoSize size);
        // user related methods.
        [FlickrMethod("flickr.people.findByEmail")]
        string GetNsidByEmail(string email);
        [FlickrMethod("flickr.people.findByUsername")]
        string GetNsidByUsername(string username);
        /// <summary>
        /// Uploades a photo file with the fileName and byte[] data provided.
        /// </summary>
        /// <param name="args">visibility attributes</param>
        /// <param name="fileName">name of the file</param>
        /// <param name="photoData">content</param>
        /// <returns>photoId</returns>
        string Upload(object[] args, string fileName, byte[] photoData);
        [FlickrMethod("flickr.people.getUploadStatus")]
        People GetUploadStatus();
        [FlickrMethod("flickr.photos.setMeta")]
        bool SetMeta(string photoId, string title, string description);
    }
}
