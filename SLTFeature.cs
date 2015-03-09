using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;
using GAFEditor.Utils;

namespace Saltr.UnitySdk
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

        /// <summary>
        /// Gets the token, a unique identifier for a feature.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="saltr.SLTFeature"/> is isRequired.
        /// </summary>
        /// <value><c>true</c> if isRequired; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }

        public SLTFeatureType? FeatureType { get; set; }

        /// <summary>
        /// Gets the user defined properties.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

        #region Utils

        public override string ToString()
        {
            return string.Format(ToStringPatthern, Token, Properties);
        }

        //public Dictionary<string, object> ToDictionary()
        //{
        //    var featureDict = new Dictionary<string, object>();
        //    featureDict[SLTConstants.Token] = Token.ToUpper();
        //    Properties.RemoveEmptyOrNullEntries();
        //    featureDict[SLTConstants.Value] = Json.Serialize(Properties);
        //    return featureDict;
        //}

        #endregion Utils
    }

    public enum SLTFeatureType
    {
        Generic
    }
}
