using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Reflection;
using Linq.Flickr.Attribute;
using Linq.Flickr.Proxies;
using Linq.Flickr.Repository.Abstraction;

namespace Linq.Flickr
{
    /// <summary>
    /// Used for creating IEnumerable<typeparamref name="T"/> result from REST response.
    /// </summary>
    /// <typeparam name="T">IDisposable</typeparam>
    public class CollectionBuilder<T>
    {
        public CollectionBuilder()
        {
            _object = Activator.CreateInstance<T>();
            elementProxy = new FlickrElementProxy(new WebRequestProxy());
        }
        /// <summary>
        /// takes a root element , used if any attribute values need to be copied into all decendant nodes.
        /// </summary>
        /// <param name="rootElement"></param>
        public CollectionBuilder(string rootElement)
        {
            _object = Activator.CreateInstance<T>();
            this.rootElement = rootElement;
        }

        public void FillProperty(T obj, string name, object value)
        {
            if (_propertyMap.ContainsKey(name))
            {
                ProcessMember(name, obj, value);
            }
        }

        public delegate void ItemChangeHandler (T item);
      
        public IEnumerable<T> ToCollection(XmlElement element, ItemChangeHandler OnItemChange)
        {
            Type objectInfo = _object.GetType();

            CreatePropertyMap(_object, null);

            IList<T> list = new List<T>();

            IList<XmlElement> elements = element.Descendants(GetProcessingElement(objectInfo));

            foreach (XmlElement e in elements)
            {
                T obj = Activator.CreateInstance<T>();
                // process any attribute from root element.
                if (!string.IsNullOrEmpty(rootElement))
                {
                    XmlElement root = e.FindElement(rootElement);
                    ProcessAttribute(obj, root);
                }
              
                ProcessNode(obj, e);
                
                // raise event so that any change might take place.
                if (OnItemChange != null)
                {
                    OnItemChange(obj);
                }
                // finally add to the list.
                list.Add(obj);
            }
            return list;
        }

        public IEnumerable<T> ToCollection(string requestUrl, ItemChangeHandler OnItemChange)
        {
            XmlElement element = elementProxy.GetResponseElement(requestUrl);
            return ToCollection(element, OnItemChange);
        }

        private void ProcessMember(string name, object obj, object value)
        {
            Type info = obj.GetType();

            object[] xmlElements = info.GetCustomAttributes(typeof(XElementAttribute), true);
            object[] xmlAttributes = info.GetCustomAttributes(typeof(XAttributeAttribute), true);

            if (xmlElements.Length == 1 || xmlAttributes.Length == 1)
            {
                PropertyInfo pInfo = info.GetProperty(_propertyMap[name].PropertyName,
                                                         BindingFlags.NonPublic | BindingFlags.Instance |
                                                         BindingFlags.Public);

                if (string.Compare(_propertyMap[name].ObjectFullName, info.FullName, true, CultureInfo.InvariantCulture) == 0)
                {
                    if (pInfo.CanWrite)
                    {
                        pInfo.SetValue(obj, GetValue(pInfo.PropertyType, value), null);
                    }
                }
                else
                {
                    object childObj = pInfo.GetValue(obj, null);
                    
                    if (childObj == null)
                    {
                        childObj = Activator.CreateInstance(Type.GetType(_propertyMap[name].ObjectFullName));
                    }

                    PropertyInfo childPInfo = childObj.GetType().GetProperty(_propertyMap[name].InnerPropertyName,
                                                                             BindingFlags.NonPublic |
                                                                             BindingFlags.Instance |
                                                                             BindingFlags.Public);

                    if (childPInfo.CanWrite)
                    {
                        childPInfo.SetValue(childObj, GetValue(childPInfo.PropertyType, value), null);
                    }
                    /// set the property
                    pInfo.SetValue(obj, childObj, null);
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

        private void ProcessAttribute(T obj, XmlNode element)
        {
            if (element != null)
            {
                foreach (XmlAttribute attribute in element.Attributes)
                {
                    FillProperty(obj, attribute.LocalName, (attribute.Value ?? string.Empty));
                }
            }
        }

        private void ProcessNode(T obj, XmlElement rootElement)
        {

            IList<XmlElement> decendants = rootElement.Descendants();

            if (decendants.Count > 0)
            {
                // set the elements
                foreach (XmlElement item in decendants)
                {
                    FillProperty(obj, item.LocalName, (item.InnerText ?? string.Empty));
                }
            }
            else
            {
                // single element.
                if (!string.IsNullOrEmpty(rootElement.InnerText))
                {
                    FillProperty(obj, rootElement.LocalName, (rootElement.InnerText ?? string.Empty));
                }
            }

            ProcessAttribute(obj, rootElement);
        }

        private void CreatePropertyMap(object obj, PropertyInfo parentInfo)
        {
            PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo info in infos)
            {
                object[] attr = info.GetCustomAttributes(typeof(XNameAttribute), true);

                if (attr.Length == 1)
                {
                    ObjectWrapper wrapper = new ObjectWrapper() { ObjectFullName = obj.GetType().FullName };

                    if (parentInfo != null)
                    {
                        wrapper.InnerPropertyName = info.Name;
                        wrapper.PropertyName = parentInfo.Name;
                    }
                    else
                    {
                        wrapper.PropertyName = info.Name;
                    }
                   
                    _propertyMap.Add((attr[0] as XNameAttribute).Name, wrapper);
                }

                object[] elementAttr = info.GetCustomAttributes(typeof(XChildAttribute), true);

                if (elementAttr.Length == 1)
                {
                    object childObject = Activator.CreateInstance(info.PropertyType);

                    if (childObject != null)
                    {
                        CreatePropertyMap(childObject, info);
                    }
                }
            }
        }

        private string GetProcessingElement(Type objectInfo)
        {
            string elementName = objectInfo.Name;
            object[] customAttr = objectInfo.GetCustomAttributes(typeof(XElementAttribute), true);

            if (customAttr != null && customAttr.Length == 1)
            {
                elementName = (customAttr[0] as XElementAttribute).Name;
            }
            return elementName;
        }

        private readonly T _object;
        private readonly string rootElement = string.Empty;
        readonly IDictionary<string, ObjectWrapper> _propertyMap = new Dictionary<string, ObjectWrapper>();

        internal class ObjectWrapper
        {
            public string PropertyName { get; set; }
            public string InnerPropertyName{ get; set;}
            public string ObjectFullName { get; set;}
        }

        private IFlickrElement elementProxy;
    }
}