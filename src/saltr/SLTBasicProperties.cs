using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
{
    class SLTBasicProperties
    {
        private string _age;
		private string _gender;			//Gender "F", "M", "female", "male"
		
		private string _appVersion;		// Version of the client app, e.g. 4.1.1
		
		private string _systemName;		//The name of the OS the current device is running. E.g. iPhone OS.
		private string _systemVersion;	//The version number of the OS the current device is running. E.g. 6.0.
		
		private string _browserName;	//The name of the browser the current device is running. E.g. Chrome.
		private string _browserVersion;	//The version number of the browser the current device is running. E.g. 17.0.
		
		private string _deviceName;		//A human-readable name representing the device.
		private string _deviceType;		//The Type name of the device. E.g. iPad.
		
		private string _locale;			//The current locale the user is in. E.g. en_US.
		
		private string _contry;			//The country the user is in, specified by ISO 2-letter code. E.g. US for United States.
										//Set to (locate) to detect the country based on the IP address of the caller.
		
		private string _region;			//The region (state) the user is in. E.g. ca for California.
										//Set to (locate) to detect the region based on the IP address of the caller.
		
		private string _city;			//The city the user is in. E.g. San Francisco.
										//Set to (locate) to detect the city based on the IP address of the caller.
		
		private string _location;		//The location (latitude/longitude) of the user. E.g. 37.775,-122.4183.
										//Set to (locate) to detect the location based on the IP address of the caller.

		public SLTBasicProperties()
		{
			
		}

		/// <summary>
		/// Gets or sets the age.
		/// </summary>
        public string age
        {
            get { return _age; }
            set { _age = value; }
        }

		/// <summary>
		/// Gets or sets the gender.
		/// </summary>
		/// <value>The gender "F", "M", "female", "male".</value>
        public string gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

		/// <summary>
		/// Gets or sets the app version.
		/// </summary>
		/// <value>Version of the client app, e.g. 4.1.1.</value>
        public string appVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; }
        }

		/// <summary>
		/// Gets or sets the name of the system.
		/// </summary>
		/// <value>The name of the OS the current device is running. E.g. iPhone OS.</value>
        public string systemName
        {
            get { return _systemName; }
            set { _systemName = value; }
        }

		/// <summary>
		/// Gets or sets the system version.
		/// </summary>
		/// <value>The version number of the OS the current device is running. E.g. 6.0.</value>
        public string systemVersion
        {
            get { return _systemVersion; }
            set { _systemVersion = value; }
        }

		/// <summary>
		/// Gets or sets the name of the browser.
		/// </summary>
		/// <value>The name of the browser the current device is running. E.g. Chrome.</value>
        public string browserName
        {
            get { return _browserName; }
            set { _browserName = value; }
        }

		/// <summary>
		/// Gets or sets the browser version.
		/// </summary>
		/// <value>The version number of the browser the current device is running. E.g. 17.0.</value>
        public string browserVersion
        {
            get { return _browserVersion; }
            set { _browserVersion = value; }
        }

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		/// <value>A human-readable name representing the device.</value>
        public string deviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

		/// <summary>
		/// Gets or sets the type of the device.
		/// </summary>
		/// <value>The Type name of the device. E.g. iPad.</value>
        public string deviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }

		/// <summary>
		/// Gets or sets the locale.
		/// </summary>
		/// <value>The current locale the user is in. E.g. en_US.</value>
        public string locale
        {
            get { return _locale; }
            set { _locale = value; }
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
            get { return _contry; }
            set { _contry = value; }
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
            get { return _region; }
            set { _region = value; }
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
            get { return _city; }
            set { _city = value; }
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
            get { return _location; }
            set { _location = value; }
        }
    }
}
