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

       	public string Client
		{
			get { return RawData.GetValue<string>("CLIENT"); }
			set { RawData["CLIENT"] = value; }
		}

       	public string DeviceId
		{
			get { return RawData.GetValue<string>("deviceId"); }
			set { RawData["deviceId"] = value; }
		}

      	public string SocialId
		{
			get { return RawData.GetValue<string>("socialId"); }
			set { RawData["socialId"] = value; }
		}

//       	public string socialNetwork;

       	public string ClientKey
		{
			get { return RawData.GetValue<string>("clientKey"); }
			set { RawData["clientKey"] = value; }
		}

//       	public string appVersion;

		public List<object> DeveloperFeatures
		{
			get { return RawData.GetValue<List<object>>("developerFeatures"); }
			set { RawData["developerFeatures"] = value; }
		}

		public string ApiVersion
		{
			get { return RawData.GetValue<string>("apiVersion"); }
			set { RawData["apiVersion"] = value; }
		}

//       	public string saltrUserId;

		public bool DevMode
		{
			get { return (bool) RawData.GetValue("devMode"); }
			set { RawData["devMode"] = value; }
		}
		
		public SLTBasicProperties BasicProperties
		{
			get { return new SLTBasicProperties(RawData.GetValue("basicProperties").ToDictionaryOrNull()); }
			set { RawData["basicProperties"] = value.RawData; }
		}

		public Dictionary<string, object> CustomProperties
		{
			get { return RawData.GetValue("customProperties").ToDictionaryOrNull(); }
			set { RawData["customProperties"] = value; }
		}

		// TODO @gyln: make a different class for these? can inherit from a base that will have the common fields
		public string Id
		{
			get { return RawData.GetValue<string>("id"); }
			set { RawData["id"] = value; }
		}

//		public string type;

		public string Source
		{
			get { return RawData.GetValue<string>("source"); }
			set { RawData["source"] = value; }
		}

		public string OS
		{
			get { return RawData.GetValue<string>("os"); }
			set { RawData["os"] = value; }
		}

		public string Email
		{
			get { return RawData.GetValue<string>("email"); }
			set { RawData["email"] = value; }
		}
  	}
}
