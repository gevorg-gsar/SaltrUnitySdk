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
        #region Fields

        private string _token;
        private bool _isRequired;
        private Dictionary<string, object> _properties;

        #endregion Fields

        #region properties

        /// <summary>
		/// Gets the token, a unique identifier for a feature.
		/// </summary>
		public string Token
		{
			get { return _token; }
		}

        /// <summary>
        /// Gets a value indicating whether this <see cref="saltr.SLTFeature"/> is isRequired.
        /// </summary>
        /// <value><c>true</c> if isRequired; otherwise, <c>false</c>.</value>
        public bool Required
        {
            get { return _isRequired; }
        }

		/// <summary>
		/// Gets the user defined properties.
		/// </summary>
		public Dictionary<string,object> Properties
		{
			get { return _properties; }
		}

        #endregion properties

        #region Ctor

        public SLTFeature(string token, Dictionary<string, object> properties, bool isRequired)
        {
            Init(token, properties, isRequired);
        }

        public SLTFeature(string token, Dictionary<string, object> properties)
        {
            Init(token, properties, false);
        }

        public SLTFeature(string token)
        {
            Init(token, null, false);
        }

        #endregion Ctor

        #region Internal Methods

        private void Init(string token, Dictionary<string, object> properties, bool isRequired)
        {
            _token = token;
            _properties = properties;
            _isRequired = isRequired;
        }

        #endregion Internal Methods

        #region Utils

        public override string ToString()
        {
            return "Feature { token : " + _token + " , value : " + _properties + "}";
        }

		public Dictionary<string, object> ToDictionary()
		{
			var featureDict = new Dictionary<string, object>();
			featureDict["token"] = _token.ToUpper();
			_properties.RemoveEmptyOrNullEntries();
			featureDict["value"] = MiniJSON.Json.Serialize(_properties);
			return featureDict;
        }

        #endregion Utils
    }
}
