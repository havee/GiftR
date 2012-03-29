using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Linq.Flickr
{
    /// <summary>
    /// Provides extension methods for processing REST data.
    /// </summary>
    public static class RestExtension
    {
        public static IList<XmlElement> Descendants(this XmlElement element, string nodeName)
        {
            XmlNodeList list = element.GetElementsByTagName(nodeName);

            IList<XmlElement> elements = new List<XmlElement>();

            foreach (XmlNode node in list)
            {
                if (node is XmlElement)
                {
                    elements.Add((XmlElement)node);                     
                }
            }
            return elements;
        }

        public static IList<XmlElement> Descendants(this XmlElement element)
        {
            IList<XmlElement> elements = new List<XmlElement>();

            XmlNode node = element.FirstChild;

            while(node!= null)
            {
                if (node is XmlElement)
                {
                    elements.Add(node as XmlElement);
                }
                node = node.NextSibling;
            }

            return elements;
        }

        public static XmlElement Load(XmlReader reader)
        {
            XmlDocument document = new XmlDocument();
            document.Load(reader);
            return document.DocumentElement;
        }

        public static XmlElement Element(this XmlElement element, string name)
        {
            XmlNodeList nodes = element.GetElementsByTagName(name);

            if (nodes.Count == 1)
            {
                return nodes[0] as XmlElement;
            }
            return null;
        }

        public static XmlAttribute Attribute(this XmlElement element, string name)
        {
            foreach (XmlAttribute att in element.Attributes)
            {
                if (string.Compare(att.LocalName, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return att;
                }
            }
            return null;
        }

        public static void Save(this XmlElement element, TextWriter stream)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(element.Value);
            doc.Save(stream);
        }

        public static XmlElement Parse(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }

        public static XmlElement ValidateResponse(this XmlElement element)
        {
            if (string.Compare(element.Attribute("stat").Value, "ok", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return element;
            }
            var error = (from e  in element.Descendants("err")
                         select new
                                    {
                                        Code =    e.Attribute("code").Value,
                                        Message = e.Attribute("msg").Value
                                    }).Single();
            throw new FlickrException(error.Code, error.Message);
        }

        public static XmlElement FindElement(this XmlElement element, string nodeName)
        {
            XmlNodeList list = element.OwnerDocument.GetElementsByTagName(nodeName);

            if (list.Count == 1)
                return list[0] as XmlElement;

            return null;
        }
    }
}