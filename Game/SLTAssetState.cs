using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// Represents a state of an asset.
    /// </summary>
    public class SLTAssetState
    {
        #region Properties

        /// <summary>
        /// Gets the token, a unique identifier for each state of an asset.
        /// </summary>
        public string Token
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the properties, associated with the state.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLTAssetState(string token, Dictionary<string, object> properties)
        {
            Token = token;
            Properties = properties;
        }

        #endregion Ctor

    }
}
