using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
	/// <summary>
	/// Represents a particular instance of an asset, placed on board.
	/// </summary>
    public class SLTAssetInstance
    {
		
		private string _token;
		private List<SLTAssetState> _states;
        private object _properties;
       
        internal SLTAssetInstance(string Token, List<SLTAssetState> states, object Properties)
        {
            _token = Token;
            _states = states;
            _properties = Properties;
        }

		/// <summary>
		/// Gets the token, a unique identifier for each asset, not unique for a particular instance
		/// </summary>
		public string token
		{
			get { return _token; }
		}

		/// <summary>
		/// Gets the states the instance is in.
		/// </summary>
		public List<SLTAssetState> states
		{
			get { return _states; }
		}

		/// <summary>
		/// Gets the asset properties.
		/// </summary>
		public object properties
		{
			get { return _properties; }
		}
    }
}
