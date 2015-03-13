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
        public string Token {get;set;}

        /// <summary>
        /// Gets the X coordinate of the pivot relative to the top left corner, in pixels.
        /// </summary>
        public float PivotX { get; set; }

        /// <summary>
        /// Gets the Y coordinate of the pivot relative to the top left corner, in pixels.
        /// </summary>
        public float PivotY { get; set; }

        /// <summary>
        /// Gets the properties, associated with the state.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

    }
}
