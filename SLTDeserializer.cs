using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Game;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk
{
    public class SLTDeserializer
    {
        #region Ctor

        public SLTDeserializer()
        { }

        #endregion Ctor

        #region Business Methods

        public static List<SLTExperiment> DecodeExperiments(Dictionary<string, object> rootNode)
        {
            if (rootNode == null)
            {
                return new List<SLTExperiment>();
            }

            List<SLTExperiment> experiments = new List<SLTExperiment>();
            Dictionary<string, object> rootDictionary = rootNode; //  (Dictionary<string, object>)rootNod["responseData"];

            if (rootDictionary == null)
            {
                return null;
            }

            if (rootDictionary.ContainsKey("experiments"))
            {
                IEnumerable<object> experimentDictionaryList = (IEnumerable<object>)rootDictionary["experiments"];
                foreach (var experimentDictionaryObj in experimentDictionaryList)
                {
                    Dictionary<string, object> experimentDictionary = (Dictionary<string, object>)experimentDictionaryObj;

                    string token = "";
                    string partition = "";
                    SLTExperimentType experimentType = SLTExperimentType.Unknown;
                    IEnumerable<object> customEvents = null;

                    if (experimentDictionary.ContainsKey("token"))
                        token = experimentDictionary["token"].ToString();

                    if (experimentDictionary.ContainsKey("partition"))
                        partition = experimentDictionary["partition"].ToString();

                    if (experimentDictionary.ContainsKey("type"))
                        experimentType = (SLTExperimentType)Enum.Parse(typeof(SLTExperimentType), experimentDictionary["type"].ToString(), true);

                    if (experimentDictionary.ContainsKey("customEventList"))
                        customEvents = (IEnumerable<object>)experimentDictionary["customEventList"];

                    SLTExperiment experimentInfo = new SLTExperiment(token, partition, experimentType, customEvents);
                    experiments.Add(experimentInfo);
                }
            }

            return experiments;
        }

        public static Dictionary<string, SLTFeature> DecodeFeatures(Dictionary<string, object> rootNode)
        {
            if (rootNode == null)
            {
                return new Dictionary<string, SLTFeature>();
            }

            Dictionary<string, SLTFeature> features = new Dictionary<string, SLTFeature>();
            if (rootNode.ContainsKey("features"))
            {
                IEnumerable<object> featureDictionaryList = (IEnumerable<object>)rootNode["features"];

                foreach (var featureDictionary in featureDictionaryList)
                {
                    Dictionary<string, object> featureNod = (Dictionary<string, object>)featureDictionary;
                    string tokken = "";
                    if (featureNod.ContainsKey("token"))
                    {
                        tokken = featureNod["token"].ToString();
                    }

                    Dictionary<string, object> properties = new Dictionary<string, object>();
                    //TODO @GSAR: remove "data" check later when API versioning is done.
                    if (featureNod.ContainsKey("data"))
                    {
                        properties = (Dictionary<string, object>)featureNod["data"];
                    }
                    else if (featureNod.ContainsKey("properties"))
                    {
                        properties = (Dictionary<string, object>)featureNod["properties"];
                    }

                    bool isRequired = false;
                    if (featureNod.ContainsKey("required"))
                    {
                        isRequired = featureNod["required"].ToString() == "true";
                    }

                    features[tokken] = new SLTFeature(tokken, properties, isRequired);
                }
            }

            return features;
        }

        public static List<SLTLevelPack> DecodeLevels(Dictionary<string, object> rootNod)
        {
            if (rootNod == null)
            {
                return new List<SLTLevelPack>();
            }

            SLTLevelType levelType = SLTLevelType.Matching;
            if (rootNod.ContainsKey("levelType"))
            {
                levelType = (SLTLevelType)Enum.Parse(typeof(SLTLevelType), rootNod["levelType"].ToString(), true);
            }

            List<SLTLevelPack> levelPacks = new List<SLTLevelPack>();
            Dictionary<string, object> rootDictionary = rootNod;  // rootNod["responseData"].toDictionaryOrNull();
            if (rootDictionary == null)
            {
                return null;
            }

            int index = -1;
            if (rootDictionary.ContainsKey("levelPacks"))
            {
                IEnumerable<object> levelPackDictionaryList = (IEnumerable<object>)rootDictionary["levelPacks"];

                foreach (var LevelPackDictionaryObj in levelPackDictionaryList)
                {
                    Dictionary<string, object> levelPackDictionary = (Dictionary<string, object>)LevelPackDictionaryObj;

                    string token = "";
                    if (levelPackDictionary.ContainsKey("token"))
                    {
                        token = (string)levelPackDictionary["token"];
                    }

                    int packIndex = 0;
                    if (levelPackDictionary.ContainsKey("index"))
                    {
                        int.TryParse(levelPackDictionary["index"].ToString(), out packIndex);
                    }

                    List<SLTLevel> levelStructures = new List<SLTLevel>();

                    IEnumerable<object> leveldictionaryList = null;
                    if (levelPackDictionary.ContainsKey("levels"))
                    {
                        leveldictionaryList = (IEnumerable<object>)levelPackDictionary["levels"];
                    }

                    object properties = null;
                    foreach (var levelobj in leveldictionaryList)
                    {
                        ++index;
                        Dictionary<string, object> levelDict = (Dictionary<string, object>)levelobj;

                        int id = 0;
                        if (levelDict.ContainsKey("id"))
                        {
                            int.TryParse(levelDict["id"].ToString(), out id);
                        }

                        int ind = 0;
                        if (levelDict.ContainsKey("index"))
                        {
                            int.TryParse(levelDict["index"].ToString(), out ind);
                        }

                        string url = "";
                        if (levelDict.ContainsKey("url"))
                        {
                            url = levelDict["url"].ToString();
                        }

                        int version = 0;
                        if (levelDict.ContainsKey("version"))
                        {
                            int.TryParse(levelDict["version"].ToString(), out version);
                        }

                        // if (levelDict.ContainsKey("index"))
                        //   packIndex = Int32.Parse(levelDict["index"].ToString());

                        if (levelDict.ContainsKey("properties"))
                        {
                            properties = levelDict["properties"];
                        }

                        int localIndex = 0;
                        if (levelDict.ContainsKey("localIndex"))
                        {
                            localIndex = Int32.Parse(levelDict["localIndex"].ToString());
                        }
                        else if (levelDict.ContainsKey("index"))
                        {
                            localIndex = Int32.Parse(levelDict["index"].ToString());
                        }

                        //TODO @GSAR: later, leave localIndex only!
                        levelStructures.Add(new SLTLevel(id.ToString(), levelType, index, localIndex, packIndex, url, properties as Dictionary<string, object>, version.ToString()));
                    }

                    //TODO @GSAR: remove this sort when SALTR confirms correct ordering
                    levelStructures.Sort(new SLTLevel.SortByIndex());
                    //levelStructures.Sort(new SLTLevel.SortByIndex());
                    SLTLevelPack levelPack = new SLTLevelPack(token, packIndex, levelStructures);
                    levelPacks.Add(levelPack);

                }
            }
            //TODO @GSAR: remove this sort when SALTR confirms correct ordering
            levelPacks.Sort(new SLTLevelPack.SortByIndex());
            return levelPacks;
        }

        #endregion Business Methods

    }
}
