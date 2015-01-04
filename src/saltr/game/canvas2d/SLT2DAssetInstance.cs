using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace saltr.game.canvas2d
{
	/// <summary>
	/// Represents an instance of a 2D asset on board
	/// </summary>
    public class SLT2DAssetInstance : SLTAssetInstance
    {
        private float _x;

		/// <summary>
		/// Gets the X coordinate.
		/// </summary>
        public float x
        {
            get { return _x; }
        }

        private float _y;

		/// <summary>
		/// Gets the Y coordinate.
		/// </summary>
        public float y
        {
            get { return _y; }
        }

        private float _rotation;

		/// <summary>
		/// Gets the rotation.
		/// </summary>
        public float Rotation
        {
            get { return _rotation; }
        }


        internal SLT2DAssetInstance(string token, List<SLTAssetState> states, object properties, float x, float y, float rotaition)
            : base(token, states, properties)
        {
            _x = x;
            _y = y;
            _rotation = rotaition;
           
        }

		/// <summary>
		/// Gets the position as a Unity standard vector, for convenience.
		/// </summary>
        public Vector2 Position { 
			get { return new Vector2(_x, _y); } 
		}
    }
}
