using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
	/// <summary>
	/// An iterator for <see cref="saltr.Game.Matching.SLTCells"/>.
	/// </summary>
    public class SLTCellsIterator
    {
        #region Fields

        private uint _vectorLength;
        private int _currentPosition;

        private SLTCells _cells;

        #endregion Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="saltr.Game.Matching.SLTCellsIterator"/> class.
        /// </summary>
        /// <param name="cells"><see cref="saltr.Game.Matching.SLTCells"/> object to traverse</param>
        public SLTCellsIterator(SLTCells cells)
        {
            _cells = cells;
            Reset();
        }

        #endregion Ctor

        #region Business Methods

        /// <summary>
        /// Indicates wheather there are any more cells remaining to traverse.
        /// </summary>
        /// <returns><c>true</c>, if there are any cells remaining to traverse, <c>false</c> otherwise.</returns>
        public bool HasNext()
        {
            return _currentPosition != _vectorLength;
        }

        /// <summary>
        /// Advances the iterator.
        /// </summary>
        /// <returns> The next cell, prior advancing.</returns>
        public SLTCell Next()
        {
            return _cells.RawData[_currentPosition++];
        }

        /// <summary>
        /// Resets the iterator.
        /// </summary>
        public void Reset()
        {
            _vectorLength = (uint)_cells.RawData.Count();
            _currentPosition = 0;
        }

        #endregion Business Methods
        
    }
}
