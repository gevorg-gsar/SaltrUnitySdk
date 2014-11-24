using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
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

		void Init(string token, object Properties, bool Required)
		{
			_token = token;
			_properties = Properties;
			_required = Required;
		}

        public SLTFeature(string token, object Properties, bool Required)
        {
			Init(token, Properties, Required);
        }

		public SLTFeature(string token, object Properties)
		{
			Init(token, Properties, false);
		}

		public SLTFeature(string token)
		{
			Init(token, null, false);
		}

        public override string ToString()
        {
            return "Feature { token : " + _token + " , value : " + _properties + "}";
        }
    }
}
