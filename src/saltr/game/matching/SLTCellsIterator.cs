using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game.matching
{
	/// <summary>
	/// An iterator for <see cref="saltr.game.matching.SLTCells"/>.
	/// </summary>
    public class SLTCellsIterator
    {
        public SLTCells _cells;
        public uint _vectorLength;
        public int _currentPosition;

		/// <summary>
		/// Initializes a new instance of the <see cref="saltr.game.matching.SLTCellsIterator"/> class.
		/// </summary>
		/// <param name="cells"><see cref="saltr.game.matching.SLTCells"/> object to traverse</param>
        public SLTCellsIterator(SLTCells cells)
        {
            _cells = cells;
            reset();
        }

		/// <summary>
		/// Indicates wheather there are any more cells remaining to traverse.
		/// </summary>
		/// <returns><c>true</c>, if there are any cells remaining to traverse, <c>false</c> otherwise.</returns>
        public bool hasNext()
        {
            return _currentPosition != _vectorLength;
        }

		/// <summary>
		/// Advances the iterator.
		/// </summary>
		/// <returns> The next cell, prior advancing.</returns>
        public SLTCell next()
        {
            return _cells.rawData[_currentPosition++];
        }

		/// <summary>
		/// Resets the iterator.
		/// </summary>
        public void reset()
        {
            _vectorLength = (uint)_cells.rawData.Count();
            _currentPosition = 0;
        }
    }
}
