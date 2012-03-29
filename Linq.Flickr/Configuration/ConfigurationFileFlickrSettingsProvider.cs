using System.Configuration;

namespace Linq.Flickr.Configuration
{
    public class ConfigurationFileFlickrSettingsProvider : IFlickrSettingsProvider
    {
        public FlickrSettings GetCurrentFlickrSettings()
        {
            if (settings == null)
            {
                settings = (FlickrSettings)ConfigurationManager.GetSection("flickr");
            }

            return settings;
        }

        private FlickrSettings settings;
    }
}