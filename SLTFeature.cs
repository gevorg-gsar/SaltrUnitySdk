using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk
{
	/// <summary>
	/// Represents an application feature - a uniquely identifiable set of properties.
	/// </summary>
    public class SLTFeature
    {
        private string _token;
        private Dictionary<string,object> _properties;
        private bool _required;
		
		/// <summary>
		/// Gets the token, a unique identifier for a feature.
		/// </summary>
		public string Token
		{
			get { return _token; }
		}

		/// <summary>
		/// Gets the user defined properties.
		/// </summary>
		public Dictionary<string,object> Properties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="saltr.SLTFeature"/> is isRequired.
		/// </summary>
		/// <value><c>true</c> if isRequired; otherwise, <c>false</c>.</value>
        public bool Required
        {
            get { return _required; }
        }

		void Init(string token, Dictionary<string,object> Properties, bool Required)
		{
			_token = token;
			_properties = Properties;
			_required = Required;
		}

		internal SLTFeature(string token, Dictionary<string,object> Properties, bool Required)
		{
			Init(token, Properties, Required);
		}

		internal SLTFeature(string token, Dictionary<string,object> Properties)
		{
			Init(token, Properties, false);
		}

		internal SLTFeature(string token)
		{
			Init(token, null, false);
		}

        public override string ToString()
        {
            return "Feature { token : " + _token + " , value : " + _properties + "}";
        }

		internal Dictionary<string, object> ToDictionary()
		{
			var ret = new Dictionary<string, object>();
			ret["token"] = _token.ToUpper();
			_properties.RemoveEmptyOrNull();
			ret["value"] = MiniJSON.Json.Serialize(_properties);
			return ret;
		}
    }
}
