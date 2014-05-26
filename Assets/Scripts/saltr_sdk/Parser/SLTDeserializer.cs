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

        public static List<SLTExperiment> decodeExperiments(Dictionary<string, object> rootNod)
        {
            if (rootNod == null)
                return new List<SLTExperiment>();
            List<SLTExperiment> experiments = new List<SLTExperiment>();

            Dictionary<string, object> rootDictionary = (Dictionary<string, object>)rootNod["responseData"];

            if (rootDictionary == null)
                return null;

            //TODO @daal. supporting partitionName(old) and partition.
            if (rootDictionary.ContainsKey("experimentInfo"))
            {
                IEnumerable<object> experimentDictionaryList = (IEnumerable<object>)rootDictionary["experimentInfo"];
                foreach (var experimentDictionaryObj in experimentDictionaryList)
                {
                    Dictionary<string, object> experimentDictionary = (Dictionary<string, object>)experimentDictionaryObj;

                    string token = "";
                    string partition = "";
                    string type = "";
                    string trackingType = "";

                    if (experimentDictionary.ContainsKey("token"))
                        token = experimentDictionary["token"].ToString();

                    if (experimentDictionary.ContainsKey("partition"))
                        partition = experimentDictionary["partition"].ToString();

                    if (experimentDictionary.ContainsKey("type"))
                        type = experimentDictionary["type"].ToString();

                    if (trackingType.Contains("trackingType"))
                        trackingType = experimentDictionary["trackingType"].ToString();

                    IEnumerable<object> customEvents = new List<object>();

                    if (experimentDictionary.ContainsKey("customEventList"))
                    {
                        customEvents = (IEnumerable<object>)experimentDictionary["customEventList"];
                    }
                    SLTExperiment experimentInfo = new SLTExperiment(token, partition, type, customEvents.ToList());
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
            Dictionary<string, object> rootDictionary = (Dictionary<string, object>)rootNod["responseData"];

            if (rootDictionary == null)
                return null;

            if (rootDictionary.ContainsKey("features"))
            {
                IEnumerable<object> featureDictionaryList = (IEnumerable<object>)rootDictionary["features"];

                foreach (var featureDictionaryObj in featureDictionaryList)
                {
                    foreach (var featureDictionary in featureDictionaryList)
                    {
                        Dictionary<string, object> featureNod = (Dictionary<string, object>)featureDictionary;
                        string tokken = "";
                        if (featureNod.ContainsKey("token"))
                            tokken = featureNod["token"].ToString();

                        Dictionary<string, object> properties = new Dictionary<string, object>();

                        if (featureNod.ContainsKey("data"))
                            properties = (Dictionary<string, object>)featureNod["data"];

                        features[tokken] = new SLTFeature(tokken, properties);
                    }
                }
            }

            return features;
        }


        public static List<SLTLevelPack> decodeLevels(Dictionary<string, object> rootNod)
        {
            if (rootNod == null)
                return new List<SLTLevelPack>();

            List<SLTLevelPack> levelPacks = new List<SLTLevelPack>();

            Dictionary<string, object> rootDictionary = rootNod["responseData"].toDictionaryOrNull();
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
                        levelPackDictionary["index"].toIntegerOrZero();

                    List<SLTLevel> levelStructureList = new List<SLTLevel>();

                    IEnumerable<object> leveldictionaryList = null;
                    if (levelPackDictionary.ContainsKey("levels"))
                        leveldictionaryList = (IEnumerable<object>)levelPackDictionary["levels"];

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


                        object prop = null;
                        if (levelDict.ContainsKey("properties"))
                            prop = levelDict["properties"];

                        SLTLevel lv = new SLTLevel(id.ToString(), ind, url, prop, version.ToString());
                        if (lv != null)
                            levelStructureList.Add(lv);
                    }

                    levelStructureList.Sort(new saltr_unity_sdk.SLTLevel.SortById());
                    SLTLevelPack levelPack = new SLTLevelPack(tokken, index, levelStructureList);
                    levelPacks.Add(levelPack);

                }
            }

            levelPacks.Sort(new saltr_unity_sdk.SLTLevelPack.SortByIndex());
            return levelPacks;
        }
    }
}
