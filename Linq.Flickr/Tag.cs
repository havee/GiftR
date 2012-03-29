using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqExtender.Attribute;
using LinqExtender;
using LinqExtender.Interface;

namespace Linq.Flickr
{
    public enum  TagListMode
    {
        Popular,
        PhotoSpecific
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "tag", Namespace = "", IsNullable = false)]
    public class Tag : PopularTag, IQueryObject, ITagText
    {
        public Tag()
        {
            
        }

        public Tag(PopularTag pTag)
        {
            base.Score = pTag.Score;
            base.Text = pTag.Text;
        }

        [UniqueIdentifier]
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute(AttributeName = "id", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Id { get; set; }
        [Ignore]
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute(AttributeName = "author", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Author { get; set; }
        [Ignore]
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute(AttributeName = "authorname", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AuthorName { get; set; }
        [Ignore]
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute(AttributeName = "raw", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Raw { get; set; }

        /// <summary>
        /// Use to query tag for photo.
        /// </summary>
        [OriginalFieldName("photo_id")]
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore]
        public string PhotoId { get; set; }
        /// <summary>
        ///Specifies, what type of tags list to get.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public TagListMode ListMode { get; set; }

        [Ignore]
        public string[] Tags
        {
            set
            {
                Text = string.Join(",", value);
            }
        }
    }
}
