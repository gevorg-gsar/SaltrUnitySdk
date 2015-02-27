using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using GAFEditor.Utils;

namespace Saltr.UnitySdk.Repository
{
    public class SLTDummyRepository : ISLTRepository
    {
        #region Ctor

        public SLTDummyRepository()
        {
        }

        #endregion Ctor

        #region Business Methods

        //TODO: Gor Implement methods below.
        
        public void CacheObject(string name, string version, object obj)
        {
            
        }

        public object GetObjectFromApplication(string fileName)
        {
            return Json.Deserialize(Resources.Load<TextAsset>(fileName).text);
        }

        public object GetObjectFromCache(string fileName)
        {
            return null;
        }

        public object GetObjectFromStorage(string name)
        {
            return null;
        }

        public string GetObjectVersion(string name)
        {
            return string.Empty;
        }

        public void SaveObject(string name, object obj)
        {

        }
        #endregion Business Methods

        

        

        
    }
}
