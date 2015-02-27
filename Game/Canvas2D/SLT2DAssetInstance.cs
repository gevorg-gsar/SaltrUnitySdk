using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Saltr.UnitySdk.Game.Canvas2D
{
    /// <summary>
    /// Represents an instance of a 2D asset on board
    /// </summary>
    public class SLT2DAssetInstance : SLTAssetInstance
    {
        #region Properties
        
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public float X
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public float Y
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        public float Rotation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the position as a Unity standard vector, for convenience.
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }

        #endregion Properties

        #region Ctor

        public SLT2DAssetInstance(string token, List<SLTAssetState> states, object properties, float x, float y, float rotaition)
            : base(token, states, properties)
        {
            X = x;
            Y = y;
            Rotation = rotaition;
        }

        #endregion Ctor
    }
}
