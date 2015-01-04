using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using saltr.utils;

namespace saltr.game
{

    internal abstract class SLTLevelParser
    {


        public abstract Dictionary<string, object> ParseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap);

		public Dictionary<string, object> ParseLevelAssets(Dictionary<string, object> rootNode)
		{ 
			if (!rootNode.ContainsKey ("assets"))
				return new Dictionary<string, object>();
			
			Dictionary<string, object> assetsNodes = rootNode["assets"].ToDictionaryOrNull();
			
			Dictionary<string, object> assetMap = new Dictionary<string, object>();
			foreach (var assetId in assetsNodes.Keys)
			{
				if(assetsNodes.ContainsKey(assetId.ToString()))
					assetMap[assetId.ToString()] = ParseAsset(assetsNodes[assetId.ToString()].ToDictionaryOrNull());
			}
			return assetMap;
		}

		//Parsing assets here
		private SLTAsset ParseAsset(Dictionary<string, object> assetNode)
		{
			string token = "";
			object properties = null;
			Dictionary<string, object> States = new Dictionary<string, object>();
			
			if (assetNode.ContainsKey("properties"))
				
				properties = assetNode["properties"];
			
			if (assetNode.ContainsKey("states"))
			{
				States = ParseAssetStates(assetNode["states"].ToDictionaryOrNull());
			}
			
			if (assetNode.ContainsKey("token"))
			{
				token = assetNode["token"].ToString();
			}
			
			return new SLTAsset(token, properties, States);
		}

		private Dictionary<string, object> ParseAssetStates(Dictionary<string, object> stateNodes)
		{
			Dictionary<string, object> statesMap = new Dictionary<string, object>();
			foreach (var stateId in stateNodes.Keys)
			{
				statesMap[stateId.ToString()] = ParseAssetState(stateNodes[stateId.ToString()].ToDictionaryOrNull());
			}
			
			return statesMap;
		}

        protected virtual SLTAssetState ParseAssetState(Dictionary<string, object> stateNode)
        {
            string token = String.Empty;
            Dictionary<string, object> properties = new Dictionary<string, object>();

            if (stateNode.ContainsKey("token"))
            {
                token = stateNode["token"].ToString();
            }

            if (stateNode.ContainsKey("properties"))
            {
                properties = stateNode["properties"].ToDictionaryOrNull();
            }

            return new SLTAssetState(token, properties);
        }		
    }
}