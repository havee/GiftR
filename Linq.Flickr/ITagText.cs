using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqExtender.Attribute;
using LinqExtender.Interface;

namespace Linq.Flickr
{
    /// <summary>
    /// Contains the text for the Tag, optionally use it for adding new photo tags.
    /// </summary>
    public interface ITagText
    {
        /// <summary>
        /// Sets the tag values.
        /// </summary>
        string[] Tags { set;}
        string PhotoId { get; set; }
    }
}
