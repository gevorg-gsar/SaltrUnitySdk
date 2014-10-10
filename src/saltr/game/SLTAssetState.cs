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
		public string _token;
		public Dictionary<string, object> _properties;

     	public  SLTAssetState(string Token, Dictionary<string,object> Properties)
       	{
           _token = Token;
           _properties = Properties;
       	}

		/// <summary>
		/// Gets the token, a unique identifier for each state of an asset.
		/// </summary>
		public string token
		{
			get{return _token;}
		}

		/// <summary>
		/// Gets the properties, associated with the state.
		/// </summary>
		public Dictionary<string,object> properties
		{
			get{return _properties;}
		}

    }
}
