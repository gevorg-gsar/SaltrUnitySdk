using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saltr.game;
using saltr.utils;

namespace saltr
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
		
        public static Dictionary<string, SLTFeature> decodeFeatures(Dictionary<string, object> rootNode)
        {
            if (rootNode == null)
				return new Dictionary<string, SLTFeature>();
			Dictionary<string, SLTFeature> features = new Dictionary<string, SLTFeature>();



			if (rootNode.ContainsKey("features"))
            {
				IEnumerable<object> featureDictionaryList = (IEnumerable<object>)rootNode["features"];

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
					else if (featureNod.ContainsKey("properties"))
						properties = (Dictionary<string, object>)featureNod["properties"];

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
			
			if (rootNod.ContainsKey("levelType"))
			{
				levelType = rootNod["levelType"].ToString();
			}
			
			
			
			List<SLTLevelPack> levelPacks = new List<SLTLevelPack>();
			
			Dictionary<string, object> rootDictionary = rootNod;  // rootNod["responseData"].toDictionaryOrNull();
			if (rootDictionary == null)
				return null;
			int index = -1;
			if (rootDictionary.ContainsKey("levelPacks"))
			{
				IEnumerable<object> levelPackDictionaryList = (IEnumerable<object>)rootDictionary["levelPacks"];
				
				foreach (var LevelPackDictionaryObj in levelPackDictionaryList)
				{
					Dictionary<string, object> levelPackDictionary = (Dictionary<string, object>)LevelPackDictionaryObj;
					
					string tokken = "";
					if (levelPackDictionary.ContainsKey("token"))
						tokken = (string)levelPackDictionary["token"];
					
					
					int packIndex = 0;
					if (levelPackDictionary.ContainsKey("index"))
						packIndex = levelPackDictionary["index"].toIntegerOrZero();
					
					List<SLTLevel> levelStructureList = new List<SLTLevel>();
					
					IEnumerable<object> leveldictionaryList = null;
					if (levelPackDictionary.ContainsKey("levels"))
						leveldictionaryList = (IEnumerable<object>)levelPackDictionary["levels"];
					
					
					object prop = null;
					
					
					foreach (var levelobj in leveldictionaryList)
					{
						++index;
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
						
						//TODO @GSAR: later, leave localIndex only!w
						levelStructureList.Add(new SLTLevel(id.ToString(), levelType, index, localIndex, packIndex, url, prop.toDictionaryOrNull(), version.ToString()));
					}

					//TODO @GSAR: remove this sort when SALTR confirms correct ordering
					levelStructureList.Sort(new SLTLevel.SortByIndex());
					//levelStructureList.Sort(new SLTLevel.SortByIndex());
					SLTLevelPack levelPack = new SLTLevelPack(tokken, packIndex, levelStructureList);
					levelPacks.Add(levelPack);
					
				}
			}
			//TODO @GSAR: remove this sort when SALTR confirms correct ordering
			levelPacks.Sort(new SLTLevelPack.SortByIndex());
			return levelPacks;
		}
    }
}
