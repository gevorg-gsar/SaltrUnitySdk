using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using saltr.utils;

namespace saltr.game
{

    public abstract class SLTLevelParser
    {


        public abstract Dictionary<string, object> parseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap);

		public Dictionary<string, object> parseLevelAssets(Dictionary<string, object> rootNode)
		{ 
			if (!rootNode.ContainsKey ("assets"))
				return new Dictionary<string, object>();
			
			Dictionary<string, object> assetsNodes = rootNode["assets"].toDictionaryOrNull();
			
			Dictionary<string, object> assetMap = new Dictionary<string, object>();
			foreach (var assetId in assetsNodes.Keys)
			{
				if(assetsNodes.ContainsKey(assetId.ToString()))
					assetMap[assetId.ToString()] = parseAsset(assetsNodes[assetId.ToString()].toDictionaryOrNull());
			}
			return assetMap;
		}

		//Parsing assets here
		private SLTAsset parseAsset(Dictionary<string, object> assetNode)
		{
			string token = "";
			object properties = null;
			Dictionary<string, object> States = new Dictionary<string, object>();
			
			if (assetNode.ContainsKey("properties"))
				
				properties = assetNode["properties"];
			
			if (assetNode.ContainsKey("states"))
			{
				States = parseAssetStates(assetNode["states"].toDictionaryOrNull());
			}
			
			if (assetNode.ContainsKey("token"))
			{
				token = assetNode["token"].ToString();
			}
			
			return new SLTAsset(token, properties, States);
		}

		private Dictionary<string, object> parseAssetStates(Dictionary<string, object> stateNodes)
		{
			Dictionary<string, object> statesMap = new Dictionary<string, object>();
			foreach (var stateId in stateNodes.Keys)
			{
				statesMap[stateId.ToString()] = parseAssetState(stateNodes[stateId.ToString()].toDictionaryOrNull());
			}
			
			return statesMap;
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
    }
}