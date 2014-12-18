using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.resource
{
   	internal class SLTRequestArguments
 	{
       	public string CLIENT;

       	public string deviceId;

      	public string socialId;

//       	public string socialNetwork;

       	public string clientKey;

//       	public string appVersion;

       	public List<object> developerFeatures;

       	public string apiVersion;

//       	public string saltrUserId;

		public bool devMode;
		
		public SLTBasicProperties basicProperties;
		public Dictionary<string, object> customProperties;

		// TODO @gyln: make a different class for these? can inherit from a base that will have the common fields
		public string id; 

//		public string type;

		public string source;

		public string os;

		public string email;
  	}
}
