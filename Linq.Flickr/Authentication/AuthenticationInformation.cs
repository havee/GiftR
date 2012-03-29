using Linq.Flickr.Configuration;

namespace Linq.Flickr.Authentication
{
    public class AuthInfo
    {
        public AuthToken AuthToken { get; set; }

        public FlickrSettings FlickrSettings { get; set; }
    }
}