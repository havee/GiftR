using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Linq.Flickr
{
    public abstract class XmlToObject<T>
    {
        public static T Deserialize(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new XmlTextReader(new StringReader(xml)));
            }
            catch
            {
                return default(T);
            }
        }

        public static string Serialize(T element)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, element);
            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}