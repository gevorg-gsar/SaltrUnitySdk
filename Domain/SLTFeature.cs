using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;
//using GAFEditor.Utils;

namespace Saltr.UnitySdk.Domain
{
    /// <summary>
    /// Represents an application feature - a uniquely identifiable set of properties.
    /// </summary>
    public class SLTFeature
    {
        #region Constants

        private const string ToStringPatthern = @"Feature {{ token : {0} , value : {1}}}";

        #endregion Constants

        #region Properties

        public string Token { get; set; }

        public bool? IsRequired { get; set; }

        public SLTFeatureType? FeatureType { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

        #region Utils

        public override string ToString()
        {
            return string.Format(ToStringPatthern, Token, Properties);
        }

        #endregion Utils
    }

    public enum SLTFeatureType
    {
        Generic
    }
}
