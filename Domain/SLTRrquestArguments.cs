using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain
{
    public class SLTRequestArguments
    {
        #region Fields

        private Dictionary<string, object> _rawData;

        #endregion Fields

        #region Ctor

        public SLTRequestArguments()
        {
            _rawData = new Dictionary<string, object>();
        }

        #endregion Ctor

        #region Properties

        public Dictionary<string, object> RawData
        {
            get { return _rawData; }
        }

        public string Client
        {
            get { return RawData.GetValue<string>("CLIENT"); }
            set { RawData["CLIENT"] = value; }
        }

        public string DeviceId
        {
            get { return RawData.GetValue<string>(SLTConstants.DeviceId); }
            set { RawData[SLTConstants.DeviceId] = value; }
        }

        public string SocialId
        {
            get { return RawData.GetValue<string>(SLTConstants.SocialId); }
            set { RawData[SLTConstants.SocialId] = value; }
        }

        public string ClientKey
        {
            get { return RawData.GetValue<string>(SLTConstants.UrlParamClientKey); }
            set { RawData[SLTConstants.UrlParamClientKey] = value; }
        }

        public List<SLTFeature> DeveloperFeatures
        {
            get { return RawData.GetValue<List<SLTFeature>>(SLTConstants.DeveloperFeatures); }
            set { RawData[SLTConstants.DeveloperFeatures] = value; }
        }

        public string ApiVersion
        {
            get { return RawData.GetValue<string>(SLTConstants.ApiVersion); }
            set { RawData[SLTConstants.ApiVersion] = value; }
        }

        public bool IsDevMode
        {
            get { return (bool)RawData.GetValue(SLTConstants.UrlParamDevMode); }
            set { RawData[SLTConstants.UrlParamDevMode] = value; }
        }

        public SLTBasicProperties BasicProperties
        {
            get { return new SLTBasicProperties(RawData.GetValue(SLTConstants.BasicProperties) as Dictionary<string, object>); }
            set { RawData[SLTConstants.BasicProperties] = value.RawData; }
        }

        public Dictionary<string, object> CustomProperties
        {
            get { return RawData.GetValue(SLTConstants.CustomProperties) as Dictionary<string, object>; }
            set { RawData[SLTConstants.CustomProperties] = value; }
        }

        // TODO @gyln: make a different class for these? can inherit from a base that will have the common fields
        public string Id
        {
            get { return RawData.GetValue<string>(SLTConstants.Id); }
            set { RawData[SLTConstants.Id] = value; }
        }

        public string Source
        {
            get { return RawData.GetValue<string>(SLTConstants.Source); }
            set { RawData[SLTConstants.Source] = value; }
        }

        public string OS
        {
            get { return RawData.GetValue<string>(SLTConstants.OS); }
            set { RawData[SLTConstants.OS] = value; }
        }

        public string Email
        {
            get { return RawData.GetValue<string>(SLTConstants.Email); }
            set { RawData[SLTConstants.Email] = value; }
        }

        #endregion Properties

    }
}
