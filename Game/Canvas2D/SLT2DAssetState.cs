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
		// TODO @gyln: I think one also needs the width and the height of the asset state from Saltr to be able to properly utilize this information, since those might not correspond to the actual in-game asset sizes. Alternative pivot coordinates can be represented with ratios from 0 to 1 instead of pixels. 
		private float _pivotX;
		private float _pivotY;

		/// <summary>
		/// Gets the X coordinate of the pivot relative to the top left corner, in pixels.
		/// </summary>
        public float PivotX
        {
            get { return _pivotX; }
        }

		/// <summary>
		/// Gets the Y coordinate of the pivot relative to the top left corner, in pixels.
		/// </summary>
        public float PivotY
        {
            get { return _pivotY; }
        }

        internal SLT2DAssetState(string token, Dictionary<string, object> properties, float pivotX, float pivotY)
            : base(token, properties)
        {
            _pivotX = pivotX;
            _pivotY = pivotY;
        }
    }
}
