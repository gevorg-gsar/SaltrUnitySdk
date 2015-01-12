﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
	/// <summary>
	/// A simple data structure, providing convenient access to board cells.
	/// </summary>
    public class SLTCells
    {
        private int _width;
        private int _height;
        private SLTCell[] _rawData;
        private SLTCellsIterator _iterator;

		/// <summary>
		/// Gets the width - the number of columns.
		/// </summary>
        public int Width
        {
            get { return _width; }
        }

		/// <summary>
		/// Gets the height - the number of rows.
		/// </summary>
        public int Height
        {
            get { return _height; }
        }

		/// <summary>
		/// Gets the underlying array containing all the cells.
		/// </summary>
        public SLTCell[] RawData
        {
            get { return _rawData; }
        }

		/// <summary>
		/// Gets the default iterator, attached to this instance, which is created on first usage, and remains the same ever since.
		/// </summary>
        public SLTCellsIterator Iterator
        {
            get
            {
                if (_iterator == null)
                {
                    _iterator = new SLTCellsIterator(this);
                }
                return _iterator;
            }
        }

        internal SLTCells(int width, int height)
        {
            _width = width;
            _height = height;
            Allocate();
        }

        private void Allocate()
        {
            _rawData = new SLTCell[_width * _height];
        }

		internal void Insert(int col, int row, SLTCell cell)
        {
            if (_rawData.Count() <= row * _width + col)
                Array.Resize(ref this._rawData, row * _width + col + 1);

            _rawData[(row * _width) + col] = cell;
        }

		/// <summary>
		/// Retrieves the cell specified by column and row.
		/// </summary>
		/// <param name="col">The column of the cell.</param>
		/// <param name="row">The row of the cell.</param>
		public SLTCell Retrieve(int col, int row)
        {
			if (_rawData.Count() <= row * _width + col)
				Array.Resize(ref this._rawData, row * _width + col + 1);

			return _rawData[(row * _width) + col];
        }

		/// <summary>
		/// Creates and returns an iterator, attached to this instance.
		/// </summary>
        public SLTCellsIterator GetIterator()
        {
            _iterator = new SLTCellsIterator(this);
            return _iterator;
        }
    }
}
