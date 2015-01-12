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
        private string _token;
        private List<SLTLevel> _levels;
        private int _index;
        

        internal SLTLevelPack(string token, int index, List<SLTLevel> levels)
        {
            _token = token;
            _index = index;
            _levels = levels;
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

		/// <summary>
		/// Gets the index of the pack.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>
		/// Returns the token.
		/// </summary>
        public override string ToString()
        {
            return _token;
        }

		internal void Dispose()
		{
			// We are NOT disposing levels here as they still can be used by the app (references!).
			// We let levels to be garbage collected later if not used.
			_levels.Clear();
			_levels = null;
		}

        internal class SortByIndex : IComparer<SLTLevelPack>
        {
            public int Compare(SLTLevelPack x, SLTLevelPack y)
            {
                if (x == null && y != null)
                    return -1;

                if (x != null && y == null)
                    return 1;

                if (x == null && y == null)
                    return 1;


                if (x.Index > y.Index)
                    return 1;

                if (x.Index < y.Index)
                    return -1;
                return 1;
            }
        }

    }
}
