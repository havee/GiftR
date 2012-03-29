using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linq.Flickr
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "token", Namespace = "", IsNullable = false)]
    public class AuthToken
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "id", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Id { get; set; }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "perm", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Perm { get; set; }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "userId", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UserId { get; set; }
        /// <summary>
        /// Full name of the user.
        /// </summary>
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "fullname", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FullName { get; set; }
    }
}
