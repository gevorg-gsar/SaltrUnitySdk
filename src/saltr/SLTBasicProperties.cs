using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saltr.utils;

namespace saltr
{
	/// <summary>
	/// The SLTBasicProperties class represents the basic user properties.
	/// This information is useful for analytics and statistics.
	/// </summary>
	public class SLTBasicProperties // TODO @gyln: get rid of alll the strings 
    {
		internal Dictionary<string, object> RawData;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public SLTBasicProperties()
		{
			RawData = new Dictionary<string, object>();
		}

		internal SLTBasicProperties(Dictionary<string, object> rawData)
		{
			RawData = rawData;
		}

		/// <summary>
		/// The age of the user.
		/// </summary>
        public string Age
        {
			get { return RawData.GetValue<string>("age"); }
			set { RawData["age"] = value; }
        }

		/// <summary>
		/// The gender information of the user.
		/// Possible values are "f" (female) and "m" (male)
		/// </summary>
        public string Gender
		{
			get { return RawData.GetValue<string>("gender"); }
			set { RawData["gender"] = value; }
		}

		/// <summary>
		/// Version of the client app, e.g. 4.1.1.
		/// </summary>
        public string AppVersion
		{
			get { return RawData.GetValue<string>("appVersion"); }
			set { RawData["appVersion"] = value; }
		}

		/// <summary>
		/// The name of the OS the current device is running. E.g. iPhone OS.
		/// </summary>
        public string SystemName
		{
			get { return RawData.GetValue<string>("systemName"); }
			set { RawData["systemName"] = value; }
		}

		/// <summary>
		/// The version number of the OS the current device is running. E.g. 6.0.
		/// </summary>
        public string SystemVersion
		{
			get { return RawData.GetValue<string>("systemVersion"); }
			set { RawData["systemVersion"] = value; }
		}

		/// <summary>
		/// The name of the browser the current device is running. E.g. Chrome.
		/// </summary>
        public string BrowserName
		{
			get { return RawData.GetValue<string>("browserName"); }
			set { RawData["browserName"] = value; }
		}

		/// <summary>
		/// The version number of the browser the current device is running. E.g. 17.0.
		/// </summary>
        public string BrowserVersion
		{
			get { return RawData.GetValue<string>("browserVersion"); }
			set { RawData["browserVersion"] = value; }
		}

		/// <summary>
		/// A human-readable name representing the device.
		/// </summary>
        public string DeviceName
		{
			get { return RawData.GetValue<string>("deviceName"); }
			set { RawData["deviceName"] = value; }
		}

		/// <summary>
		/// The Type name of the device. E.g. iPad.
		/// </summary>
        public string DeviceType
		{
			get { return RawData.GetValue<string>("deviceType"); }
			set { RawData["deviceType"] = value; }
		}

		/// <summary>
		/// The current locale the user is in. E.g. en_US.
		/// </summary>
        public string Locale
		{
			get { return RawData.GetValue<string>("locale"); }
			set { RawData["locale"] = value; }
		}

		/// <summary>
		/// The country the user is in, specified by ISO 2-letter code. E.g. US for United States. 
		/// Set to (locate) to detect the country based on the IP address of the caller.
		/// </summary>
        public string Contry
		{
			get { return RawData.GetValue<string>("contry"); }
			set { RawData["contry"] = value; }
		}

		/// <summary>
		/// The region (state) the user is in. E.g. ca for California.
		/// Set to (locate) to detect the region based on the IP address of the caller.
		/// </summary>
        public string Region
		{
			get { return RawData.GetValue<string>("region"); }
			set { RawData["region"] = value; }
		}

		/// <summary>
		/// The city the user is in. E.g. San Francisco.
		/// Set to (locate) to detect the city based on the IP address of the caller.
		/// </summary>
        public string City
		{
			get { return RawData.GetValue<string>("city"); }
			set { RawData["city"] = value; }
		}

		/// <summary>
		/// The location (latitude/longitude) of the user. E.g. 37.775,-122.4183.
		/// Set to (locate) to detect the location based on the IP address of the caller.
		/// </summary>
        public string Location
		{
			get { return RawData.GetValue<string>("location"); }
			set { RawData["location"] = value; }
		}
    }
}
