using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Canvas2D
{
	/// <summary>
	/// Represents a state of a 2D asset
	/// </summary>
    public class SLT2DAssetState : SLTAssetState
    {
        #region Properties

        /// <summary>
        /// Gets the X coordinate of the pivot relative to the top left corner, in pixels.
        /// </summary>
        public float PivotX
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Y coordinate of the pivot relative to the top left corner, in pixels.
        /// </summary>
        public float PivotY
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLT2DAssetState(string token, Dictionary<string, object> properties, float pivotX, float pivotY)
            : base(token, properties)
        {
            PivotX = pivotX;
            PivotY = pivotY;
        }

        #endregion Ctor
        
    }
}
