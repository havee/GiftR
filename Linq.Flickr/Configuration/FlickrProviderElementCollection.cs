using System.Configuration;

namespace Linq.Flickr.Configuration
{
    /// <summary>
    /// contains the configured provider collection.
    /// </summary>
    public class FlickrProviderElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FlickrProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FlickrProviderElement) element).Name;
        }
        /// <summary>
        /// gets element though its name property.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new FlickrProviderElement this[string name]
        {
            get
            {
                return BaseGet(name) as FlickrProviderElement;
            }
        }
    }
}