using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTCellsIterator
    {
        public SLTCells _cells;
        public uint _vectorLength;
        public int _currentPosition;

        public SLTCellsIterator(SLTCells cells)
        {
            _cells = cells;
            reset();
        }

        public bool hasNext()
        {
            return _currentPosition != _vectorLength;
        }

        public SLTCell next()
        {
            return _cells.rawData[_currentPosition++];
        }

        public void reset()
        {
            _vectorLength = (uint)_cells.rawData.Count();
            _currentPosition = 0;
        }
    }
}
