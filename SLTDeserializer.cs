using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain;
using Saltr.UnitySdk.Game;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk
{
    public static class SLTDeserializer
    {
        #region Business Methods

        public static SLTAppData DeserializeAppData(Dictionary<string, object> responseAppData)
        {
            SLTAppData sltAppData = new SLTAppData();
            try
            {
                if (responseAppData.ContainsKey(SLTConstants.Success))
                {
                    sltAppData.Success = bool.Parse(responseAppData[SLTConstants.Success].ToString());
                }

                if (responseAppData.ContainsKey(SLTConstants.LevelType))
                {
                    sltAppData.LevelType = (SLTLevelType)Enum.Parse(typeof(SLTLevelType), responseAppData[SLTConstants.LevelType].ToString(), true);
                }

                if (responseAppData.ContainsKey(SLTConstants.Features))
                {
                    sltAppData.Features = DeserializeFeatures(responseAppData[SLTConstants.Features] as IEnumerable<object>);
                }

                if (responseAppData.ContainsKey(SLTConstants.Experiments))
                {
                    sltAppData.Experiments = DeserializeExperiments(responseAppData[SLTConstants.Experiments] as IEnumerable<object>);
                }

                if (responseAppData.ContainsKey(SLTConstants.LevelPacks))
                {
                    sltAppData.LevelPacks = DeserializeLevelPacks(responseAppData[SLTConstants.LevelPacks] as IEnumerable<object>);
                }
            }
            catch
            {
                throw;
            }
            return sltAppData;
        }

        public static SLTFeature DeserializeFeature(Dictionary<string, object> responseFeature)
        {
            SLTFeature sltFeature = new SLTFeature();
            try
            {
                if (responseFeature.ContainsKey(SLTConstants.Token))
                {
                    sltFeature.Token = responseFeature[SLTConstants.Token].ToString();
                }

                if (responseFeature.ContainsKey(SLTConstants.FeatureType))
                {
                    sltFeature.FeatureType = (SLTFeatureType)Enum.Parse(typeof(SLTFeatureType), responseFeature[SLTConstants.FeatureType].ToString(), true);
                }

                if (responseFeature.ContainsKey(SLTConstants.Properties))
                {
                    sltFeature.Properties = responseFeature[SLTConstants.Properties] as Dictionary<string, object>;
                }
            }
            catch
            {
                throw;
            }

            return sltFeature;

        }

        public static List<SLTFeature> DeserializeFeatures(IEnumerable<object> responseFeatures)
        {
            List<SLTFeature> features = new List<SLTFeature>();
            try
            {
                if (responseFeatures != null)
                {
                    foreach (Dictionary<string, object> responseFeature in responseFeatures)
                    {
                        features.Add(DeserializeFeature(responseFeature));
                    }
                }

            }
            catch
            {
                throw;
            }
            return features;
        }

        public static SLTExperiment DeserializeExperiment(Dictionary<string, object> responseExperiment)
        {
            SLTExperiment sltExperiment = new SLTExperiment();
            try
            {
                if (responseExperiment.ContainsKey(SLTConstants.Token))
                {
                    sltExperiment.Token = responseExperiment[SLTConstants.Token].ToString();
                }

                if (responseExperiment.ContainsKey(SLTConstants.Partition))
                {
                    sltExperiment.Partition = responseExperiment[SLTConstants.Partition].ToString();
                }

                if (responseExperiment.ContainsKey(SLTConstants.Type))
                {
                    sltExperiment.ExperimentType = (SLTExperimentType)Enum.Parse(typeof(SLTExperimentType), responseExperiment[SLTConstants.Type].ToString(), true);
                }

                if (responseExperiment.ContainsKey(SLTConstants.CustomEventList))
                {
                    sltExperiment.CustomEvents = responseExperiment[SLTConstants.CustomEventList] as IEnumerable<object>;
                }
            }
            catch
            {
                throw;
            }

            return sltExperiment;
        }

        public static List<SLTExperiment> DeserializeExperiments(IEnumerable<object> responseExperiments)
        {
            List<SLTExperiment> experiments = new List<SLTExperiment>();
            try
            {
                if (responseExperiments != null)
                {
                    foreach (Dictionary<string, object> responseExperiment in responseExperiments)
                    {
                        experiments.Add(DeserializeExperiment(responseExperiment));
                    }
                }
            }
            catch
            {
                throw;
            }

            return experiments;
        }

        public static SLTLevelPack DeserializeLevelPack(Dictionary<string, object> responseLevelPack)
        {
            SLTLevelPack sltLevelPack = new SLTLevelPack();
            try
            {

                if (responseLevelPack.ContainsKey(SLTConstants.Token))
                {
                    sltLevelPack.Token = responseLevelPack[SLTConstants.Token].ToString();
                }

                if (responseLevelPack.ContainsKey(SLTConstants.Name))
                {
                    sltLevelPack.Name = responseLevelPack[SLTConstants.Name].ToString();
                }

                if (responseLevelPack.ContainsKey(SLTConstants.Id))
                {
                    sltLevelPack.Id = int.Parse(responseLevelPack[SLTConstants.Id].ToString());
                }

                if (responseLevelPack.ContainsKey(SLTConstants.Index))
                {
                    sltLevelPack.Index = int.Parse(responseLevelPack[SLTConstants.Index].ToString());
                }

                if (responseLevelPack.ContainsKey(SLTConstants.Levels))
                {
                    sltLevelPack.Levels = DeserializeLevels(responseLevelPack[SLTConstants.Levels] as IEnumerable<object>);
                }
            }
            catch
            {
                throw;
            }

            return sltLevelPack;
        }

        public static List<SLTLevelPack> DeserializeLevelPacks(IEnumerable<object> responseLevelPacks)
        {
            List<SLTLevelPack> levelPacks = new List<SLTLevelPack>();
            try
            {
                if (responseLevelPacks != null)
                {
                    foreach (Dictionary<string, object> responseLevelPack in responseLevelPacks)
                    {
                        levelPacks.Add(DeserializeLevelPack(responseLevelPack));
                    }
                }
            }
            catch
            {
                throw;
            }

            return levelPacks;
        }

        public static SLTLevel DeserializeLevel(Dictionary<string, object> responseLevel)
        {
            SLTLevel sltLevel = new SLTLevel();
            try
            {
                if (responseLevel.ContainsKey(SLTConstants.Url))
                {
                    sltLevel.Url = responseLevel[SLTConstants.Url].ToString();
                }

                if (responseLevel.ContainsKey(SLTConstants.Id))
                {
                    sltLevel.Id = int.Parse(responseLevel[SLTConstants.Id].ToString());
                }

                if (responseLevel.ContainsKey(SLTConstants.Index))
                {
                    sltLevel.Index = int.Parse(responseLevel[SLTConstants.Index].ToString());
                }

                if (responseLevel.ContainsKey(SLTConstants.LocalIndex))
                {
                    sltLevel.LocalIndex = int.Parse(responseLevel[SLTConstants.LocalIndex].ToString());
                }

                if (responseLevel.ContainsKey(SLTConstants.Version))
                {
                    sltLevel.Version = int.Parse(responseLevel[SLTConstants.Version].ToString());
                }

                if (responseLevel.ContainsKey(SLTConstants.VariationVersion))
                {
                    sltLevel.VariationVersion = int.Parse(responseLevel[SLTConstants.VariationVersion].ToString());
                }

                if (responseLevel.ContainsKey(SLTConstants.VariationId))
                {
                    sltLevel.VariationId = int.Parse(responseLevel[SLTConstants.VariationId].ToString());
                }

                if (responseLevel.ContainsKey(SLTConstants.Properties))
                {
                    sltLevel.Properties = responseLevel[SLTConstants.Properties] as Dictionary<string, object>;
                }
            }
            catch
            {
                throw;
            }

            return sltLevel;
        }

        public static List<SLTLevel> DeserializeLevels(IEnumerable<object> responseLevels)
        {
            List<SLTLevel> levels = new List<SLTLevel>();
            try
            {
                if (responseLevels != null)
                {
                    foreach (Dictionary<string, object> responseLevel in responseLevels)
                    {
                        levels.Add(DeserializeLevel(responseLevel));
                    }
                }
            }
            catch
            {
                throw;
            }

            return levels;
        }

        public static SLTLevelContent DeserializeLevelContent(Dictionary<string, object> responseLevelContent)
        {
            SLTLevelContent sltLevelContent = new SLTLevelContent();
            try
            {
                if (responseLevelContent.ContainsKey(SLTConstants.Properties))
                {
                    sltLevelContent.Properties = responseLevelContent[SLTConstants.Properties] as Dictionary<string, object>;
                } 
            }
            catch
            {
                throw;
            }
            return sltLevelContent;
        }

        #endregion Business Methods

    }
}
