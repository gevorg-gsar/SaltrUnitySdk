﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
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
		public string token
		{
			get { return _token; }
		}

		/// <summary>
		/// Gets the user defined properties.
		/// </summary>
		public Dictionary<string,object> properties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="saltr.SLTFeature"/> is required.
		/// </summary>
		/// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool required
        {
            get { return _required; }
        }

		void Init(string token, Dictionary<string,object> Properties, bool Required)
		{
			_token = token;
			_properties = Properties;
			_required = Required;
		}

		public SLTFeature(string token, Dictionary<string,object> Properties, bool Required)
		{
			Init(token, Properties, Required);
		}

		public SLTFeature(string token, Dictionary<string,object> Properties)
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
