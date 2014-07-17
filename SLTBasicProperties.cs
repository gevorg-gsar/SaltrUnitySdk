using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    class SLTBasicProperties
    {
        private uint _age;

        public uint age
        {
            get { return _age; }
            set { _age = value; }
        }


        private string _gender;

        public string gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        private string _appVersion;

        public string appVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; }
        }


        private string _systemName;

        public string systemName
        {
            get { return _systemName; }
            set { _systemName = value; }
        }


        private string _systemVersion;

        public string systemVersion
        {
            get { return _systemVersion; }
            set { _systemVersion = value; }
        }

        private string _browserName;

        public string browserName
        {
            get { return _browserName; }
            set { _browserName = value; }
        }


        private string _browserVersion;

        public string browserVersion
        {
            get { return _browserVersion; }
            set { _browserVersion = value; }
        }

        private string _deviceName;

        public string deviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }


        private string _deviceType;

        public string deviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }

        private string _locale;

        public string locale
        {
            get { return _locale; }
            set { _locale = value; }
        }


        private string _contry;

        public string contry
        {
            get { return _contry; }
            set { _contry = value; }
        }

        private string _regiom;

        public string regiom
        {
            get { return _regiom; }
            set { _regiom = value; }
        }


        private string _city;

        public string city
        {
            get { return _city; }
            set { _city = value; }
        }

        private string _location;

        public string location
        {
            get { return _location; }
            set { _location = value; }
        }

        public SLTBasicProperties()
        {

        }

    }
}
