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
        public string age
        {
			get { return RawData.getValue<string>("age"); }
			set { RawData["age"] = value; }
        }

		/// <summary>
		/// The gender information of the user.
		/// Possible values are "f" (female) and "m" (male)
		/// </summary>
        public string gender
		{
			get { return RawData.getValue<string>("gender"); }
			set { RawData["gender"] = value; }
		}

		/// <summary>
		/// Gets or sets the app version.
		/// </summary>
		/// <value>Version of the client app, e.g. 4.1.1.</value>
        public string appVersion
		{
			get { return RawData.getValue<string>("appVersion"); }
			set { RawData["appVersion"] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the system.
		/// </summary>
		/// <value>The name of the OS the current device is running. E.g. iPhone OS.</value>
        public string systemName
		{
			get { return RawData.getValue<string>("systemName"); }
			set { RawData["systemName"] = value; }
		}

		/// <summary>
		/// Gets or sets the system version.
		/// </summary>
		/// <value>The version number of the OS the current device is running. E.g. 6.0.</value>
        public string systemVersion
		{
			get { return RawData.getValue<string>("systemVersion"); }
			set { RawData["systemVersion"] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the browser.
		/// </summary>
		/// <value>The name of the browser the current device is running. E.g. Chrome.</value>
        public string browserName
		{
			get { return RawData.getValue<string>("browserName"); }
			set { RawData["browserName"] = value; }
		}

		/// <summary>
		/// Gets or sets the browser version.
		/// </summary>
		/// <value>The version number of the browser the current device is running. E.g. 17.0.</value>
        public string browserVersion
		{
			get { return RawData.getValue<string>("browserVersion"); }
			set { RawData["browserVersion"] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		/// <value>A human-readable name representing the device.</value>
        public string deviceName
		{
			get { return RawData.getValue<string>("deviceName"); }
			set { RawData["deviceName"] = value; }
		}

		/// <summary>
		/// Gets or sets the type of the device.
		/// </summary>
		/// <value>The Type name of the device. E.g. iPad.</value>
        public string deviceType
		{
			get { return RawData.getValue<string>("deviceType"); }
			set { RawData["deviceType"] = value; }
		}

		/// <summary>
		/// Gets or sets the locale.
		/// </summary>
		/// <value>The current locale the user is in. E.g. en_US.</value>
        public string locale
		{
			get { return RawData.getValue<string>("locale"); }
			set { RawData["locale"] = value; }
		}

		/// <summary>
		/// Gets or sets the contry.
		/// </summary>
		/// <value>
		/// The country the user is in, specified by ISO 2-letter code. E.g. US for United States. 
		/// Set to (locate) to detect the country based on the IP address of the caller.
		/// </value>
        public string contry
		{
			get { return RawData.getValue<string>("contry"); }
			set { RawData["contry"] = value; }
		}

		/// <summary>
		/// Gets or sets the region.
		/// </summary>
		/// <value>
		/// The region (state) the user is in. E.g. ca for California.
		/// Set to (locate) to detect the region based on the IP address of the caller.
		/// </value>
        public string region
		{
			get { return RawData.getValue<string>("region"); }
			set { RawData["region"] = value; }
		}

		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>
		/// The city the user is in. E.g. San Francisco.
		/// Set to (locate) to detect the city based on the IP address of the caller.
		/// </value>
        public string city
		{
			get { return RawData.getValue<string>("city"); }
			set { RawData["city"] = value; }
		}

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>
		/// The location (latitude/longitude) of the user. E.g. 37.775,-122.4183.
		/// Set to (locate) to detect the location based on the IP address of the caller.
		/// </value>
        public string location
		{
			get { return RawData.getValue<string>("location"); }
			set { RawData["location"] = value; }
		}
    }
}
