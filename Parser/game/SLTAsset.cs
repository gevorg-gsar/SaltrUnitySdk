using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTAsset
    {
        private string _token;
        private object _properties;
        private Dictionary<string, object> _states;

        public Dictionary<string, object> states
        {
            get { return _states; }
            set { _states = value; }
        }

        public string token
        {
            get { return _token; }
        }

        public object properties
        {
            get { return _properties; }
        }

        public SLTAsset(string token, object properties, Dictionary<string,object> States)
        {
            _token = token;
            _properties = properties;
            _states = States;
        }

        public override string ToString()
        {
            return "[Asset] type: " + _token + ", " + " keys: " + _properties;
        }
    }
}
