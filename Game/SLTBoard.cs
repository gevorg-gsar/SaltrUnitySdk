using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Saltr.UnitySdk.Game
{
	/// <summary>
	/// The SLTBoard class represents a game board. 
	/// </summary>
   	public class SLTBoard
    {
        private Dictionary<string, object> _properties;
        private List<SLTBoardLayer> _layers;
		
        internal SLTBoard(List<SLTBoardLayer> layers, Dictionary<string, object> properties)
        {
            _properties = properties;
            _layers = layers;

        }

		/// <summary>
		/// Gets the properties associated with the board.
		/// </summary>
		public Dictionary<string, object> Properties
		{
			get { return _properties; }
		}

		/// <summary>
		/// Gets the layers of the board.
		/// </summary>
		public List<SLTBoardLayer> Layers
		{
			get { return _layers; }
		}

		/// <summary>
		/// Regenerates contents of all layers.
		/// </summary>
		public void Regenerate()
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                SLTBoardLayer layer = _layers[i] as SLTBoardLayer;
                layer.Regenerate();
            }
        }
    }
}
