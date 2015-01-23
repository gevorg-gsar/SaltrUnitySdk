using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game
{

    internal abstract class SLTLevelParser
    {


        public abstract Dictionary<string, object> ParseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap);

        public Dictionary<string, object> ParseLevelAssets(Dictionary<string, object> rootNode)
        {
            if (!rootNode.ContainsKey("assets"))
            {
                return new Dictionary<string, object>();
            }

            Dictionary<string, object> assetsNodes = rootNode["assets"] as Dictionary<string, object>;

            Dictionary<string, object> assetMap = new Dictionary<string, object>();
            foreach (var assetId in assetsNodes.Keys)
            {
                if (assetsNodes.ContainsKey(assetId.ToString()))
                {
                    assetMap[assetId.ToString()] = ParseAsset(assetsNodes[assetId.ToString()] as Dictionary<string, object>);
                }
            }
            return assetMap;
        }

        //Parsing assets here
        private SLTAsset ParseAsset(Dictionary<string, object> assetNode)
        {
            string token = assetNode.ContainsKey("token") ? assetNode["token"].ToString() : String.Empty;
            object properties = assetNode.ContainsKey("properties") ? assetNode["properties"] : null;
            Dictionary<string, object> states = assetNode.ContainsKey("states") ? ParseAssetStates(assetNode["states"] as Dictionary<string, object>) : new Dictionary<string, object>();
            return new SLTAsset(token, properties, states);
        }

        private Dictionary<string, object> ParseAssetStates(Dictionary<string, object> stateNodes)
        {
            Dictionary<string, object> statesMap = new Dictionary<string, object>();
            foreach (var stateId in stateNodes.Keys)
            {
                statesMap[stateId.ToString()] = ParseAssetState(stateNodes[stateId.ToString()] as Dictionary<string, object>);
            }

            return statesMap;
        }

        protected virtual SLTAssetState ParseAssetState(Dictionary<string, object> stateNode)
        {
            string token = stateNode.ContainsKey("token") ? stateNode["token"].ToString() : String.Empty;
            Dictionary<string, object> properties = stateNode.ContainsKey("properties") ? stateNode["properties"] as Dictionary<string, object> : new Dictionary<string, object>();

            return new SLTAssetState(token, properties);
        }
    }
}