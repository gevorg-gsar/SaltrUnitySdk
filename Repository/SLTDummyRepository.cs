using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
//using GAFEditor.Utils;
using Newtonsoft.Json;

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

        //TODO: Gor Implement methods below.

        public void CacheObject<T>(string name, T obj, string version = null)
        {

        }

        public T GetObjectFromApplication<T>(string fileName) where T : class
        {
            return JsonConvert.DeserializeObject<T>(Resources.Load<TextAsset>(fileName).text);
        }

        public T GetObjectFromCache<T>(string fileName) where T : class
        {
            return null;
        }

        public T GetObjectFromStorage<T>(string name) where T : class
        {
            return null;
        }

        public string GetObjectVersion(string name)
        {
            return string.Empty;
        }

        public void SaveObject<T>(string name, T obj)
        {

        }

        #endregion Public Methods

    }
}
