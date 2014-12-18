using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
	/// <summary>
	/// Represents any kind of a board layer.
	/// </summary>
    public class SLTBoardLayer
    {
        private string _token;
        private int _index;

        internal SLTBoardLayer(string LayerId, int LayerIndex)
        {
            _token = LayerId;
            _index = LayerIndex;
        }

		/// <summary>
		/// Gets the token, a unique identifier for the layer within a board.
		/// </summary>
        public string token
        {
            get { return _token; }
        }

		/// <summary>
		/// Gets the index, layers are ordered by this index within a board.
		/// </summary>
        public int index
        {
            get { return _index; }
        }

		/// <summary>
		/// Regenerates layer contents. The behavior depends on the type of the layer. See in derived classes.
		/// </summary>
        public virtual void regenerate()
        {
            //override
        }
    }
}
