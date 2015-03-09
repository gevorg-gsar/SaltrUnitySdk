using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using Saltr.UnitySdk.Game;
using UnityEngine;

namespace Saltr.UnitySdk.Utils
{
    public static class JsonDataToTypedObjectMapper
    {
        public static void Map(this SLTAppData sltAppData, JsonData sltAppDataJsonData)
        {
            if (sltAppDataJsonData != null)
            {
                bool success;
                if (sltAppDataJsonData.ContainsKey(SLTConstants.Success) && bool.TryParse(sltAppDataJsonData[SLTConstants.Success].ToString(), out success))
                {
                    sltAppData.Success = success;
                }

                if (sltAppDataJsonData.ContainsKey(SLTConstants.Features))
                {
                    var sltFeature = new SLTFeature();
                    var featuresJsonData = sltAppDataJsonData[SLTConstants.Features] as ICollection;

                    if (featuresJsonData != null)
                    {
                        foreach (JsonData featureJsonData in featuresJsonData)
                        {
                            sltFeature.Map(featureJsonData);
                        }
                    }
                }
            }
        }

        public static void Map(this SLTFeature sltFeature, JsonData sltFeatureJsonData)
        {
            if (sltFeatureJsonData != null)
            {
                if (sltFeatureJsonData.ContainsKey(SLTConstants.Token))
                {
                    sltFeature.Token = sltFeatureJsonData[SLTConstants.Token].ToString();
                }

                if (sltFeatureJsonData.ContainsKey(SLTConstants.FeatureType))
                {
                    sltFeature.FeatureType = (SLTFeatureType)Enum.Parse(typeof(SLTFeatureType), sltFeatureJsonData[SLTConstants.FeatureType].ToString(), true);
                }

                if (sltFeatureJsonData.ContainsKey(SLTConstants.Properties))
                {
                    var propertiesJsonData = sltFeatureJsonData[SLTConstants.Properties];
                    if (propertiesJsonData != null)
                    {
                        //sltFeature.Properties = propertiesJsonData as IDictionary;

                        //foreach (var propertyJsonData in propertiesJsonData.Keys)
                        //{
                        //    Debug.Log(propertyJsonData.ToString());
                        //}
                    }
                }
            }
        }


        public static bool ContainsKey(this JsonData data, string key)
        {
            bool result = false;
            if (data == null)
                return result;
            if (!data.IsObject)
            {
                return result;
            }
            IDictionary tdictionary = data as IDictionary;
            if (tdictionary == null)
                return result;
            if (tdictionary.Contains(key))
            {
                result = true;
            }
            return result;
        }
    }
}
