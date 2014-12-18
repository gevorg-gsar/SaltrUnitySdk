﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game.canvas2d
{
	/// <summary>
	/// Represents a 2D board.
	/// </summary>
    public class SLT2DBoard : SLTBoard
    {

        private float _width;

		/// <summary>
		/// Gets the width of the board in pixels as is in Saltr level editor.
		/// </summary>
        public float width
        {
            get { return _width; }
        }

        private float _height;

		/// <summary>
		/// Gets the height of the board in pixels as is in Saltr level editor.
		/// </summary>
        public float height
        {
            get { return _height; }
        }


        internal SLT2DBoard(float width, float height, List<SLTBoardLayer> layers, Dictionary<string, object> properties)
            : base(layers, properties)
        {
            _width = width;
            _height = height;
        }


    }
}
