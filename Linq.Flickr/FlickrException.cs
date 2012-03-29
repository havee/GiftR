using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linq.Flickr
{
    /// <summary>
    /// Use to pass flickr related expections.
    /// </summary>
    public class FlickrException : System.Exception
    {
        public FlickrException(string code, string message) :
            base("Error code: " + code + " Message: " + message)
        { 
        }

        public FlickrException(string message) : base(message)
        {
            //intentionally left blank.
        }
    }
}
