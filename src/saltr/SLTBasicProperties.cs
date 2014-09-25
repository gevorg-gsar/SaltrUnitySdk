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

        public string age
        {
            get { return _age; }
            set { _age = value; }
        }

        public string gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        public string appVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; }
        }

        public string systemName
        {
            get { return _systemName; }
            set { _systemName = value; }
        }

        public string systemVersion
        {
            get { return _systemVersion; }
            set { _systemVersion = value; }
        }

        public string browserName
        {
            get { return _browserName; }
            set { _browserName = value; }
        }

        public string browserVersion
        {
            get { return _browserVersion; }
            set { _browserVersion = value; }
        }

        public string deviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

        public string deviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }

        public string locale
        {
            get { return _locale; }
            set { _locale = value; }
        }

        public string contry
        {
            get { return _contry; }
            set { _contry = value; }
        }

        public string region
        {
            get { return _region; }
            set { _region = value; }
        }

        public string city
        {
            get { return _city; }
            set { _city = value; }
        }

        public string location
        {
            get { return _location; }
            set { _location = value; }
        }
    }
}
