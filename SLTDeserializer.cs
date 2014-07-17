using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTDeserializer
    {
        public SLTDeserializer()
        { }

        public static List<SLTExperiment> decodeExperiments(Dictionary<string, object> rootNode)
        {
            if (rootNode == null)
                return new List<SLTExperiment>();
            List<SLTExperiment> experiments = new List<SLTExperiment>();


            Dictionary<string, object> rootDictionary = rootNode; //  (Dictionary<string, object>)rootNod["responseData"];

            if (rootDictionary == null)
                return null;

            if (rootDictionary.ContainsKey("experiments"))
            {
                IEnumerable<object> experimentDictionaryList = (IEnumerable<object>)rootDictionary["experiments"];
                foreach (var experimentDictionaryObj in experimentDictionaryList)
                {
                    Dictionary<string, object> experimentDictionary = (Dictionary<string, object>)experimentDictionaryObj;

                    string token = "";
                    string partition = "";
                    string type = "";
                    IEnumerable<object> customEvents = null;

                    if (experimentDictionary.ContainsKey("token"))
                        token = experimentDictionary["token"].ToString();

                    if (experimentDictionary.ContainsKey("partition"))
                        partition = experimentDictionary["partition"].ToString();

                    if (experimentDictionary.ContainsKey("type"))
                        type = experimentDictionary["type"].ToString();

                    if (experimentDictionary.ContainsKey("customEventList"))
                        customEvents = (IEnumerable<object>)experimentDictionary["customEventList"];



                    SLTExperiment experimentInfo = new SLTExperiment(token, partition, type, customEvents);
                    experiments.Add(experimentInfo);
                }
            }

            return experiments;
        }
		
        public static Dictionary<string, object> decodeFeatures(Dictionary<string, object> rootNod)
        {
            if (rootNod == null)
                return new Dictionary<string, object>();
            Dictionary<string, object> features = new Dictionary<string, object>();

            Dictionary<string, object> rootDictionary = rootNod;;

            if (rootDictionary == null)
                return null;

            if (rootDictionary.ContainsKey("features"))
            {
                IEnumerable<object> featureDictionaryList = (IEnumerable<object>)rootDictionary["features"];

                foreach (var featureDictionary in featureDictionaryList)
                {
                    Dictionary<string, object> featureNod = (Dictionary<string, object>)featureDictionary;
                    string tokken = "";
                    if (featureNod.ContainsKey("token"))
                        tokken = featureNod["token"].ToString();

                    Dictionary<string, object> properties = new Dictionary<string, object>();
                    //TODO @GSAR: remove "data" check later when API versioning is done.
                    if (featureNod.ContainsKey("data"))
                        properties = (Dictionary<string, object>)featureNod["data"];


                    bool required = false;
                    if (featureNod.ContainsKey("required"))
                        required = featureNod["required"].ToString() == "true";

                    features[tokken] = new SLTFeature(tokken, properties, required);
                }
            }

            return features;
        }
		
		public static List<SLTLevelPack> decodeLevels(Dictionary<string, object> rootNod)
		{
			if (rootNod == null)
				return new List<SLTLevelPack>();
			string levelType = SLTLevel.LEVEL_TYPE_MATCHING;
			
			List<string> keys = new List<string>();
			foreach (var item in rootNod.Keys)
			{
				UnityEngine.Debug.Log(item);
				keys.Add(item);
			}
			
			if (rootNod.ContainsKey("levelType"))
			{
				levelType = rootNod["levelType"].ToString();
			}
			
			
			
			List<SLTLevelPack> levelPacks = new List<SLTLevelPack>();
			
			Dictionary<string, object> rootDictionary = rootNod;  // rootNod["responseData"].toDictionaryOrNull();
			if (rootDictionary == null)
				return null;
			
			if (rootDictionary.ContainsKey("levelPacks"))
			{
				IEnumerable<object> levelPackDictionaryList = (IEnumerable<object>)rootDictionary["levelPacks"];
				
				foreach (var LevelPackDictionaryObj in levelPackDictionaryList)
				{
					Dictionary<string, object> levelPackDictionary = (Dictionary<string, object>)LevelPackDictionaryObj;
					
					string tokken = "";
					if (levelPackDictionary.ContainsKey("token"))
						tokken = (string)levelPackDictionary["token"];
					
					
					int index = 0;
					if (levelPackDictionary.ContainsKey("index"))
						index = levelPackDictionary["index"].toIntegerOrZero();
					
					List<SLTLevel> levelStructureList = new List<SLTLevel>();
					
					IEnumerable<object> leveldictionaryList = null;
					if (levelPackDictionary.ContainsKey("levels"))
						leveldictionaryList = (IEnumerable<object>)levelPackDictionary["levels"];
					
					
					object prop = null;
					
					
					foreach (var levelobj in leveldictionaryList)
					{
						Dictionary<string, object> levelDict = (Dictionary<string, object>)levelobj;
						
						int id = 0;
						if (levelDict.ContainsKey("id"))
							id = levelDict["id"].toIntegerOrZero();
						
						int ind = 0;
						if (levelDict.ContainsKey("index"))
							ind = levelDict["index"].toIntegerOrZero();
						
						
						string url = "";
						if (levelDict.ContainsKey("url"))
							url = levelDict["url"].ToString();
						
						int version = 0;
						if (levelDict.ContainsKey("version"))
							version = levelDict["version"].toIntegerOrZero();
						
						int packIndex = index;
						// if (levelDict.ContainsKey("index"))
						//   packIndex = Int32.Parse(levelDict["index"].ToString());
						
						if (levelDict.ContainsKey("properties"))
							prop = levelDict["properties"];
						
						
						int localIndex = 0;
						if (levelDict.ContainsKey("localIndex"))
							localIndex = Int32.Parse(levelDict["localIndex"].ToString());
						else
						{
							if (levelDict.ContainsKey("index"))
								localIndex = Int32.Parse(levelDict["index"].ToString());
						}
						
						
						levelStructureList.Add(createLevel(levelType, id.ToString(), index, localIndex, packIndex, url, prop.toDictionaryOrNull(), version.ToString()));
					}

					// TODO: figure out why 2 sort calls are required here, weird, but one doesn't seem to work O_o (gyln)
					levelStructureList.Sort(new SLTLevel.SortByIndex());
					levelStructureList.Sort(new SLTLevel.SortByIndex());
					SLTLevelPack levelPack = new SLTLevelPack(tokken, index, levelStructureList);
					levelPacks.Add(levelPack);
					
				}
			}
			
			levelPacks.Sort(new saltr_unity_sdk.SLTLevelPack.SortByIndex());
			return levelPacks;
		}

        private static SLTLevel createLevel(string levelType, string id, int index, int localIndex, int packIndex, string url, Dictionary<string, object> properties, string version)
        {
            switch (levelType)
            {
                case SLTLevel.LEVEL_TYPE_MATCHING:
                    return new SLTMatchingLevel(id, index, localIndex, packIndex, url, properties, version);
                    break;
                case SLTLevel.LEVEL_TYPE_2DCANVAS:
                    return new SLT2DLevel(id, index, localIndex, packIndex, url, properties, version);
                    break;
            }
            return null;
        }
    }
}
