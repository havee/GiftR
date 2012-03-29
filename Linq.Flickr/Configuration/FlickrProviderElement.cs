using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Linq.Flickr.Configuration
{
    /// <summary>
    /// Contains the detail for each provider element.
    /// </summary>
    public class FlickrProviderElement : ConfigurationElement
    {
        public FlickrProviderElement()
        {
            
        }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        public String Type
        {
            get
            {
                return (String)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
    }
}