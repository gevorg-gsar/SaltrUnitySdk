using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    public class SLTAsset
    {
        #region Properties

        public string Token
        {
            get;
            private set;
        }

        public object Properties
        {
            get;
            private set;
        }

        public Dictionary<string, object> StateMap
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLTAsset(string token, object properties, Dictionary<string, object> stateMap)
        {
            Token = token;
            Properties = properties;
            StateMap = stateMap;
        }

        #endregion Ctor

        #region Business Methods

        public override string ToString()
        {
            return "[Asset] type: " + Token + ", " + " keys: " + Properties;
        }

        public List<SLTAssetState> GetInstanceStates(IEnumerable<object> stateIds)
        {
            List<SLTAssetState> states = new List<SLTAssetState>();
            foreach (var stateId in stateIds)
            {
                SLTAssetState state = StateMap[stateId.ToString()] as SLTAssetState;

                if (state != null)
                {
                    states.Add(state);
                }
            }
            return states;
        }

        #endregion Ctor

    }
}
