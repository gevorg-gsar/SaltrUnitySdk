using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
    internal class SLTAsset
    {
        private string _token;
        private object _properties;
        private Dictionary<string, object> _stateMap;

        public SLTAsset(string token, object properties, Dictionary<string,object> StateMap)
        {
            _token = token;
            _properties = properties;
            _stateMap = StateMap;
        }

        public override string ToString()
        {
            return "[Asset] type: " + _token + ", " + " keys: " + _properties;
        }

		public string Token
		{
			get { return _token; }
		}
		
		public object Properties
		{
			get { return _properties; }
		}

		public List<SLTAssetState> GetInstanceStates(IEnumerable<object> stateIds)
		{
			List<SLTAssetState> states = new List<SLTAssetState>(); 
			foreach(object stateId in stateIds)
			{
				SLTAssetState state = _stateMap[stateId.ToString()] as SLTAssetState;

				if(state != null)
				{
					states.Add(state);
				}
			}
			return states;
		}
    }
}
