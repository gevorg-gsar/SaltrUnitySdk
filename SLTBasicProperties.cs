using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk
{
	/// <summary>
	/// The SLTBasicProperties class represents the basic user properties.
	/// This information is useful for analytics and statistics.
	/// </summary>
	public class SLTBasicProperties // TODO @gyln: get rid of alll the strings 
    {
        #region Fields

        private Dictionary<string, object> _rawData;

        #endregion Fields

        #region Properties

        public Dictionary<string, object> RawData
        {
            get { return _rawData; }
        }

        /// <summary>
        /// The age of the user.
        /// </summary>
        public string Age
        {
            get { return RawData.GetValue<string>(SLTConstants.Age); }
            set { RawData[SLTConstants.Age] = value; }
        }

        /// <summary>
        /// The gender information of the user.
        /// Possible values are "f" (female) and "m" (male)
        /// </summary>
        public string Gender
        {
            get { return RawData.GetValue<string>(SLTConstants.Gender); }
            set { RawData[SLTConstants.Gender] = value; }
        }

        /// <summary>
        /// Version of the client app, e.g. 4.1.1.
        /// </summary>
        public string AppVersion
        {
            get { return RawData.GetValue<string>(SLTConstants.AppVersion); }
            set { RawData[SLTConstants.AppVersion] = value; }
        }

        /// <summary>
        /// The name of the OS the current device is running. E.g. iPhone OS.
        /// </summary>
        public string SystemName
        {
            get { return RawData.GetValue<string>(SLTConstants.SystemName); }
            set { RawData[SLTConstants.SystemName] = value; }
        }

        /// <summary>
        /// The version number of the OS the current device is running. E.g. 6.0.
        /// </summary>
        public string SystemVersion
        {
            get { return RawData.GetValue<string>(SLTConstants.SystemVersion); }
            set { RawData[SLTConstants.SystemVersion] = value; }
        }

        /// <summary>
        /// The name of the browser the current device is running. E.g. Chrome.
        /// </summary>
        public string BrowserName
        {
            get { return RawData.GetValue<string>(SLTConstants.BrowserName); }
            set { RawData[SLTConstants.BrowserName] = value; }
        }

        /// <summary>
        /// The version number of the browser the current device is running. E.g. 17.0.
        /// </summary>
        public string BrowserVersion
        {
            get { return RawData.GetValue<string>(SLTConstants.BrowserVersion); }
            set { RawData[SLTConstants.BrowserVersion] = value; }
        }

        /// <summary>
        /// A human-readable name representing the device.
        /// </summary>
        public string DeviceName
        {
            get { return RawData.GetValue<string>(SLTConstants.DeviceName); }
            set { RawData[SLTConstants.DeviceName] = value; }
        }

        /// <summary>
        /// The Type name of the device. E.g. iPad.
        /// </summary>
        public string DeviceType
        {
            get { return RawData.GetValue<string>(SLTConstants.DeviceType); }
            set { RawData[SLTConstants.DeviceType] = value; }
        }

        /// <summary>
        /// The current locale the user is in. E.g. en_US.
        /// </summary>
        public string Locale
        {
            get { return RawData.GetValue<string>(SLTConstants.Locale); }
            set { RawData[SLTConstants.Locale] = value; }
        }

        /// <summary>
        /// The country the user is in, specified by ISO 2-letter code. E.g. US for United States. 
        /// Set to (locate) to detect the country based on the IP address of the caller.
        /// </summary>
        public string Country
        {
            get { return RawData.GetValue<string>(SLTConstants.Country); }
            set { RawData[SLTConstants.Country] = value; }
        }

        /// <summary>
        /// The region (state) the user is in. E.g. ca for California.
        /// Set to (locate) to detect the region based on the IP address of the caller.
        /// </summary>
        public string Region
        {
            get { return RawData.GetValue<string>(SLTConstants.Region); }
            set { RawData[SLTConstants.Region] = value; }
        }

        /// <summary>
        /// The city the user is in. E.g. San Francisco.
        /// Set to (locate) to detect the city based on the IP address of the caller.
        /// </summary>
        public string City
        {
            get { return RawData.GetValue<string>(SLTConstants.City); }
            set { RawData[SLTConstants.City] = value; }
        }

        /// <summary>
        /// The location (latitude/longitude) of the user. E.g. 37.775,-122.4183.
        /// Set to (locate) to detect the location based on the IP address of the caller.
        /// </summary>
        public string Location
        {
            get { return RawData.GetValue<string>(SLTConstants.Location); }
            set { RawData[SLTConstants.Location] = value; }
        }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// Class constructor.
        /// </summary>
        public SLTBasicProperties()
        {
            _rawData = new Dictionary<string, object>();
        }

        public SLTBasicProperties(Dictionary<string, object> rawData)
        {
            _rawData = rawData;
        }

        #endregion Ctor
        
    }
}
