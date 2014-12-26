using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saltr.utils;

namespace saltr.resource
{
   	internal class SLTRequestArguments // TODO @gyln: get rid of alll the strings 
 	{
		internal Dictionary <string, object> RawData;

		public SLTRequestArguments()
		{
			RawData = new Dictionary<string, object>();
		}

       	public string CLIENT
		{
			get { return RawData.getValue<string>("CLIENT"); }
			set { RawData["CLIENT"] = value; }
		}

       	public string deviceId
		{
			get { return RawData.getValue<string>("deviceId"); }
			set { RawData["deviceId"] = value; }
		}

      	public string socialId
		{
			get { return RawData.getValue<string>("socialId"); }
			set { RawData["socialId"] = value; }
		}

//       	public string socialNetwork;

       	public string clientKey
		{
			get { return RawData.getValue<string>("clientKey"); }
			set { RawData["clientKey"] = value; }
		}

//       	public string appVersion;

		public List<object> developerFeatures
		{
			get { return RawData.getValue<List<object>>("developerFeatures"); }
			set { RawData["developerFeatures"] = value; }
		}

		public string apiVersion
		{
			get { return RawData.getValue<string>("apiVersion"); }
			set { RawData["apiVersion"] = value; }
		}

//       	public string saltrUserId;

		public bool devMode
		{
			get { return (bool) RawData.getValue("devMode"); }
			set { RawData["devMode"] = value; }
		}
		
		public SLTBasicProperties basicProperties
		{
			get { return new SLTBasicProperties(RawData.getValue("basicProperties").toDictionaryOrNull()); }
			set { RawData["basicProperties"] = value.RawData; }
		}

		public Dictionary<string, object> customProperties
		{
			get { return RawData.getValue("customProperties").toDictionaryOrNull(); }
			set { RawData["customProperties"] = value; }
		}

		// TODO @gyln: make a different class for these? can inherit from a base that will have the common fields
		public string id
		{
			get { return RawData.getValue<string>("id"); }
			set { RawData["id"] = value; }
		}

//		public string type;

		public string source
		{
			get { return RawData.getValue<string>("source"); }
			set { RawData["source"] = value; }
		}

		public string os
		{
			get { return RawData.getValue<string>("os"); }
			set { RawData["os"] = value; }
		}

		public string email
		{
			get { return RawData.getValue<string>("email"); }
			set { RawData["email"] = value; }
		}
  	}
}
