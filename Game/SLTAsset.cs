using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    public class SLTAsset
    {
        #region Properties

        public string Token { get; set; }

        public Dictionary<string, object> States { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

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
                SLTAssetState state = States[stateId.ToString()] as SLTAssetState;

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
