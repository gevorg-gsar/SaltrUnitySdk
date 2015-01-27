using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
	/// <summary>
	/// The SLTAssetInstance class represents the game asset instance placed on board.
	/// It holds the unique identifier of the asset and current instance related states and properties.
	/// </summary>
    public class SLTAssetInstance
    {
        #region Properties

        /// <summary>
        /// The unique identifier of the asset, not unique for the instance, instead acts as a type.
        /// </summary>
        public string Token
        {
            get;
            private set;
        }

        /// <summary>
        /// The current instance states.
        /// </summary>
        public List<SLTAssetState> States
        {
            get;
            private set;
        }

        /// <summary>
        /// The current instance properties.
        /// </summary>
        public object Properties
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLTAssetInstance(string token, List<SLTAssetState> states, object properties)
        {
            Token = token;
            States = states;
            Properties = properties;
        }

        #endregion
    }
}
