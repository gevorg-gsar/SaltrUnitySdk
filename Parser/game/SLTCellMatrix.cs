﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTCellMatrix
    {
        private int _width;
        private int _height;
        private SLTCell[] _rawData;
        private SLTCellMatrixIterator _iterator;

        public int width
        {
            get { return _width; }
        }

        public int height
        {
            get { return _height; }
        }

        public SLTCell[] rawData
        {
            get { return _rawData; }
        }

        public SLTCellMatrixIterator iterator
        {
            get
            {
                if (_iterator == null)
                {
                    _iterator = new SLTCellMatrixIterator(this);
                }
                return _iterator;
            }
        }

        public SLTCellMatrix(int width, int height)
        {
            _width = width;
            _height = height;
            allocate();
        }

        private void allocate()
        {
            _rawData = new SLTCell[_width * _height];
        }

        public void insert(int row, int col, SLTCell cell)
        {
            if (_rawData.Count() <= row * _width + col)
                Array.Resize(ref this._rawData, row * _width + col + 1);

            _rawData[(row * _width) + col] = cell;
        }

        public SLTCell retrieve(int row, int col)
        {
            if (_rawData.Count() <= row * _width + row)
                Array.Resize(ref this._rawData, row * _width + row + 1);

            return _rawData[(row * _width) + row];
        }

        public SLTCellMatrixIterator getIterator()
        {
            _iterator = new SLTCellMatrixIterator(this);
            return _iterator;
        }
    }
}
