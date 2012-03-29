using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Linq.Flickr.Configuration {

    public class FlickrSettings: ConfigurationSection {

        [ConfigurationProperty("apiKey",DefaultValue="#yourKey#")]
        public string ApiKey {
            get {
                return (string)this["apiKey"];
            }
            set {
                this["apiKey"] = value;
            }
        }

        [ConfigurationProperty("secretKey", DefaultValue = "#yourSecretKey#")]
        public string SecretKey {
            get {
                return (string)this["secretKey"];
            }
            set {
                this["secretKey"] = value;
            }
        }

        [ConfigurationProperty("defaultAuthProvider", DefaultValue = "web", IsRequired = true)]
        public string DefaultProviderName
        {
            get
            {
                return (string)this["defaultAuthProvider"];
            }
            set
            {
                this["defaultAuthProvider"] = value;
            }
        }

        public AuthProviderElement DefaultProvider
        {
            get
            {
                return Providers[this.DefaultProviderName];
            }
        }

        [ConfigurationProperty("authProviders")]
        public AutheProviderElementCollection Providers
        {
            get
            {
                return this["authProviders"] as AutheProviderElementCollection;
            }
        }


    }
}
