using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
    public class SLTAssetInstance
    {
		
		private string _token;
		private List<SLTAssetState> _states;
        private object _properties;
       
        public SLTAssetInstance(string Token, List<SLTAssetState> state, object Properties)
        {
            _token = Token;
            _states = state;
            _properties = Properties;
        }

		
		public string token
		{
			get { return _token; }
		}

		public List<SLTAssetState> states
		{
			get { return _states; }
		}

		public object properties
		{
			get { return _properties; }
		}
    }
}
