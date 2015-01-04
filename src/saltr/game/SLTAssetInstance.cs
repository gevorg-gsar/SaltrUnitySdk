using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
	/// <summary>
	/// The SLTAssetInstance class represents the game asset instance placed on board.
	/// It holds the unique identifier of the asset and current instance related states and properties.
	/// </summary>
    public class SLTAssetInstance
    {
		
		private string _token;
		private List<SLTAssetState> _states;
        private object _properties;
       
        internal SLTAssetInstance(string token, List<SLTAssetState> states, object properties)
        {
            _token = token;
            _states = states;
            _properties = properties;
        }

		/// <summary>
		/// The unique identifier of the asset, not unique for the instance, instead acts as a type.
		/// </summary>
		public string Token
		{
			get { return _token; }
		}

		/// <summary>
		/// The current instance states.
		/// </summary>
		public List<SLTAssetState> States
		{
			get { return _states; }
		}

		/// <summary>
		/// The current instance properties.
		/// </summary>
		public object Properties
		{
			get { return _properties; }
		}
    }
}
