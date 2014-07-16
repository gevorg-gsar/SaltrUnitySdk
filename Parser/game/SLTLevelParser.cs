using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace saltr_unity_sdk
{

    public abstract class SLTLevelParser
    {


        public abstract Dictionary<string, object> parseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap);


        public Dictionary<string, object> parseLevelSettings(Dictionary<string, object> rootNode)
        {
            Dictionary<string, object> assetMap = new Dictionary<string, object>();
            if (rootNode.ContainsKey("assets"))
                assetMap = parseLevelAssets(rootNode["assets"].toDictionaryOrNull());

            return assetMap;
        }



        protected virtual SLTAssetState parseAssetState(Dictionary<string, object> stateNode)
        {
            string token = String.Empty;
            Dictionary<string, object> properties = new Dictionary<string, object>();

            if (stateNode.ContainsKey("token"))
            {
                token = stateNode["token"].ToString();
            }

            if (stateNode.ContainsKey("properties"))
            {
                properties = stateNode["properties"].toDictionaryOrNull();
            }

            return new SLTAssetState(token, properties);
        }



        private Dictionary<string, object> parseAssetStates(Dictionary<string, object> states)
        {
            Dictionary<string, object> statesMap = new Dictionary<string, object>();
            foreach (var stateId in states.Keys)
            {
                statesMap[stateId.ToString()] = parseAssetState(statesMap[stateId.ToString()].toDictionaryOrNull());
            }

            return statesMap;
        }





        public Dictionary<string, object> parseLevelAssets(Dictionary<string, object> rootNode)
		{ 
			Dictionary<string, object> assetsNodes = rootNode["assets"].toDictionaryOrNull();

            Dictionary<string, object> assetMap = new Dictionary<string, object>();
			foreach (var assetId in assetsNodes.Keys)
            {
				assetMap[assetId.ToString()] = parseAsset(assetsNodes[assetId.ToString()].toDictionaryOrNull());
            }
            return assetMap;
        }



        private SLTAsset parseAsset(Dictionary<string, object> assetNode)
        {
            string token = "";
            object properties = null;
            Dictionary<string, object> States = new Dictionary<string, object>();

            if (assetNode.ContainsKey("properties"))

                properties = assetNode["properties"];

            if (assetNode.ContainsKey("states"))
            {
                States = assetNode["states"].toDictionaryOrNull();
            }

            if (assetNode.ContainsKey("token"))
            {
                token = assetNode["token"].ToString();
            }

            return new SLTAsset(token, properties, States);
        }
    }
}