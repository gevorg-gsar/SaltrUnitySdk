using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
	/// <summary>
	/// Represents a level pack - a uniquely identifiable collection of levels.
	/// </summary>
    public class SLTLevelPack
    {
        #region Fields

        private int _index;
        private string _token;
        private List<SLTLevel> _levels;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the index of the pack.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets the token, a unique identifier for the pack.
        /// </summary>
        public string Token
        {
            get { return _token; }
        }

        /// <summary>
        /// Gets the list of levels of the pack.
        /// </summary>
        public List<SLTLevel> Levels
        {
            get { return _levels; }
        }

        #endregion Properties

        #region Ctor

        public SLTLevelPack(string token, int index, List<SLTLevel> levels)
        {
            _token = token;
            _index = index;
            _levels = levels;
        }

        #endregion Ctor

        #region Business Methods
        
        // <summary>
		/// Returns the token.
		/// </summary>
        public override string ToString()
        {
            return _token;
        }

		public void Dispose()
		{
			// We are NOT disposing levels here as they still can be used by the app (references!).
			// We let levels to be garbage collected later if not used.
			_levels.Clear();
			_levels = null;
		}

        #endregion Business Methods
        
    }
}
