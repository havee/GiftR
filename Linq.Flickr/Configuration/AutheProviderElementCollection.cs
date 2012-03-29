using System.Configuration;

namespace Linq.Flickr.Configuration
{
    /// <summary>
    /// contains the configured provider collection.
    /// </summary>
    public class AutheProviderElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthProviderElement) element).Name;
        }
        /// <summary>
        /// gets element though its name property.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new AuthProviderElement this[string name]
        {
            get
            {
                return BaseGet(name) as AuthProviderElement;
            }
        }
    }
}