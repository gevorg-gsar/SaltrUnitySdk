using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
   public class SLTAssetState
   {
		public string _token;
		public Dictionary<string, object> _properties;

     	public  SLTAssetState(string Token, Dictionary<string,object> Properties)
       	{
           _token = Token;
           _properties = Properties;
       	}

		public string token
		{
			get{return _token;}
		}

		public Dictionary<string,object> properties
		{
			get{return _properties;}
		}

    }
}
