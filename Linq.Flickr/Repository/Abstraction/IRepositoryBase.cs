using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.Flickr.Attribute;

namespace Linq.Flickr.Repository.Abstraction
{
    public interface IRepositoryBase 
    {
        [FlickrMethod("flickr.auth.getFrob")]
        string GetFrob();
        string GetSignature(string methodName, bool includeMethod, params object[] args);
        string BuildUrl(string functionName, params object[] args);
        string GetNsid(string method, string field, string value);
    }
}
