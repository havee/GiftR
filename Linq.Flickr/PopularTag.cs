using LinqExtender.Attribute;
using Linq.Flickr.Attribute;
using LinqExtender.Interface;

namespace Linq.Flickr
{
    public enum TagPeriod
    {
        Day,
        Week
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "tag", Namespace = "", IsNullable = false)]
    public class PopularTag
    {
        [System.Xml.Serialization.XmlText]
        public string Text { get; set; }
           

        [Ignore]
        [System.Xml.Serialization.XmlAttribute(AttributeName = "score", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Score { get; set; }

        public int Count { get; set; }
        public TagPeriod Period { get;set; }
    }
}
