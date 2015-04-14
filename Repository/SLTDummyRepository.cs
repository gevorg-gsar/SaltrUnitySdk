using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
//using GAFEditor.Utils;
using Newtonsoft.Json;
using Saltr.UnitySdk.Domain.InternalModel;
using Newtonsoft.Json.Converters;

namespace Saltr.UnitySdk.Repository
{
    public class SLTDummyRepository : ISLTRepository
    {
        #region Ctor

        public SLTDummyRepository()
        {
        }

        #endregion Ctor

        #region Public Methods
        
        public void SaveObjectInPersistentStorage<T>(string name, T obj)
        {

        }
        public T GetObjectFromPersistentStorage<T>(string name) where T : class
        {
            return null;
        }

        public void CacheObject<T>(string name, T obj, string version = null)
        {

        }

        public T GetObjectFromCache<T>(string fileName) where T : class
        {
            return null;
        }

        public string GetObjectVersion(string name)
        {
            return string.Empty;
        }

        public T GetObjectFromApplication<T>(string fileName) where T : class
        {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            if (textAsset != null)
            {
                return JsonConvert.DeserializeObject<T>(textAsset.text, new BoardConverter(), new SLTAssetTypeConverter(), new DictionaryConverter());
            }
            else
            {
                return default(T);
            }
        }

        #endregion Public Methods

    }
}
