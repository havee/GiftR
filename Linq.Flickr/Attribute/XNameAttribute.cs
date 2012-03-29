using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linq.Flickr.Attribute
{
    public class XNameAttribute : System.Attribute
    {
        private string _name = string.Empty;

        public XNameAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
