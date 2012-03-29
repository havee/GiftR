using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqExtender;
using System.Xml.Linq;
using System.Reflection;
using Linq.Flickr.Attribute;

namespace Linq.Flickr.Repository
{
    /// <summary>
    /// Use for creating IEnumerable<typeparamref name="T"/> result from REST requestUrl
    /// </summary>
    /// <typeparam name="T">IDisposable</typeparam>
    public class RestToCollectionBuilder<T> : HttpCallBase where T : IDisposable 
    {
        private T _object;
        private string _rootElement = string.Empty;

        IDictionary<string, string> _propertyMap = new Dictionary<string, string>();

        public RestToCollectionBuilder()
        {
            _object = Activator.CreateInstance<T>();
        }
        /// <summary>
        /// takes a root element , used if any attribute values need to be copied into all decendant nodes.
        /// </summary>
        /// <param name="rootElement"></param>
        public RestToCollectionBuilder(string rootElement)
        {
            _object = Activator.CreateInstance<T>();
            _rootElement = rootElement;
        }

        public void FillProperty(T obj, string name, object value)
        {
            if (_propertyMap.ContainsKey(name))
            {
                ProcessMember(name, obj, value);
            }
        }

        private void ProcessMember(string name, object obj, object value)
        {
            Type info = obj.GetType();

            object[] xmlElements = info.GetCustomAttributes(typeof(XElementAttribute), true);
            object[] xmlAttributes = info.GetCustomAttributes(typeof(XAttributeAttribute), true);

            if (xmlElements.Length == 1 || xmlAttributes.Length == 1)
            {
                PropertyInfo pInfo = info.GetProperty(_propertyMap[name],
                                                      BindingFlags.NonPublic | BindingFlags.Instance |
                                                      BindingFlags.Public);

                if (pInfo.CanWrite)
                {
                    pInfo.SetValue(obj, GetValue(pInfo.PropertyType, value), null);
                }
            }

        }

        private object GetValue(Type type, object value)
        {
            string sValue = (string)value;
            object retValue = value;

            switch (type.FullName)
            {
                case "System.Boolean":
                   retValue = string.IsNullOrEmpty(sValue) ? false : ((sValue == "0" || sValue == "false") ? false : true);
                   break;
                case "System.String":
                    retValue = Convert.ToString(value);
                    break;
                case "System.Int32":
                    retValue = Convert.ToInt32(value);
                    break;
                case "System.DateTime":
                    retValue = Convert.ToDateTime(value);
                    break;
            }
            return retValue;
        }

        public delegate void ItemChangeHandler (T item);
      
        public IEnumerable<T> ToCollection(XElement element, ItemChangeHandler OnItemChange)
        {
            Type objectInfo = _object.GetType();

            CreatePropertyMap(objectInfo);

            IList<T> list = new List<T>();

            var query = from item in element.Descendants(GetRootElement(objectInfo))
                        select item;

            XNode[] nodes = query.ToArray();

            foreach (XNode node in nodes)
            {
                T obj = Activator.CreateInstance<T>();

                if (nodes[0] is XElement)
                {
                    // process any attribute from root element.
                    if (!string.IsNullOrEmpty(_rootElement))
                    {
                        XElement root = element.FindElement(_rootElement);
                        ProcessAttribute(obj, root);
                    }

                    XElement rootElement = node as XElement;
                    ProcessNode(obj, rootElement);
                    
                    // raise event so that any change might take place.
                    if (OnItemChange != null)
                    {
                        OnItemChange(obj);
                    }
                    // finally add to the list.
                    list.Add(obj);
                }
            }
            return list;
        }

        private void ProcessAttribute(T obj, XElement element)
        {
            if (element != null)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    FillProperty(obj, attribute.Name.LocalName, (attribute.Value ?? string.Empty));
                }
            }
        }

        private void ProcessNode(T obj, XElement rootElement)
        {

            if (rootElement.HasElements)
            {
                // set the elements
                foreach (XElement item in rootElement.Descendants())
                {
                    FillProperty(obj, item.Name.LocalName, (item.Value ?? string.Empty));
                }
            }
            else
            {
                // single element.
                if (!string.IsNullOrEmpty(rootElement.Value))
                {
                    FillProperty(obj, rootElement.Name.LocalName, (rootElement.Value ?? string.Empty));
                }
            }

            ProcessAttribute(obj, rootElement);
        }

        public IEnumerable<T> ToCollection(string requestUrl, ItemChangeHandler OnItemChange)
        {
            XElement element = base.GetElement(requestUrl);
            return ToCollection(element, OnItemChange);
        }

        private void CreatePropertyMap(Type objectInfo)
        {
            PropertyInfo[] infos = objectInfo.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo info in infos)
            {
                object[] attr = info.GetCustomAttributes(typeof(XNameAttribute), true);

                if (attr != null && attr.Length == 1)
                {
                    _propertyMap.Add((attr[0] as XNameAttribute).Name, info.Name);
                }
            }
        }

        private string GetRootElement(Type objectInfo)
        {
            string elementName = objectInfo.Name;
            object[] customAttr = objectInfo.GetCustomAttributes(typeof(XElementAttribute), true);

            if (customAttr != null && customAttr.Length == 1)
            {
                elementName = (customAttr[0] as XElementAttribute).Name;
            }
            return elementName;
        }
    }
}
