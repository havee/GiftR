using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqExtender.Attribute;
using LinqExtender;
using Linq.Flickr.Attribute;
using LinqExtender.Interface;

namespace Linq.Flickr
{
    /// <summary>
    ///  Holds comment informaton for a photo.
    /// </summary>
    [Serializable, XElement("comment")]
    public partial class Comment : IQueryObject
    {
        public Comment()
        {
        }

        [OriginalFieldName("id"), XAttribute("id"), UniqueIdentifier]
        public string Id { get; set; }

        [OriginalFieldName("photo_id"), XAttribute("photo_id")]
        public string PhotoId { get; set; }

        [Ignore, XAttribute("permalink")]
        public string PermaLink { get; set; }
        [XElement("comment")]
        public string Text { get; set; }

        [XAttribute("datecreate")]
        internal string PDateCreated { get; set; }

        [XAttribute("author")]
        public string Author { get; set; }
        [XAttribute("authorname")]
        public string AuthorName { get; set; }

        public DateTime DateCreated
        {
            get
            {
                return PDateCreated.GetDate();
            }
        }
    }
}
