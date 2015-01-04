using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
	/// <summary>
	/// Represents a state of an asset.
	/// </summary>
   public class SLTAssetState
   {
		string _token;
		Dictionary<string, object> _properties;

     	internal SLTAssetState(string token, Dictionary<string,object> properties)
       	{
           _token = token;
           _properties = properties;
       	}

		/// <summary>
		/// Gets the token, a unique identifier for each state of an asset.
		/// </summary>
		public string Token
		{
			get{return _token;}
		}

		/// <summary>
		/// Gets the properties, associated with the state.
		/// </summary>
		public Dictionary<string,object> Properties
		{
			get{return _properties;}
		}

    }
}
