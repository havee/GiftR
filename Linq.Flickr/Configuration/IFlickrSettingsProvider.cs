namespace Linq.Flickr.Configuration
{
    public interface IFlickrSettingsProvider
    {
        FlickrSettings GetCurrentFlickrSettings();
    }
}