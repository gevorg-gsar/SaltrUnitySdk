using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTFeature
    {
        private string _token;

        public string token
        {
            get { return _token; }
        }
        private object _properties;

        public object properties
        {
            get { return _properties; }
        }

        private bool _required;

        public bool required
        {
            get { return _required; }
        }

        public SLTFeature(string token, object Properties = null, bool Required = false)
        {
            _token = token;
            _properties = Properties;
            _required = Required;
        }

        public override string ToString()
        {
            return "Feature { token : " + _token + " , value : " + _properties + "}";
        }
    }
}
