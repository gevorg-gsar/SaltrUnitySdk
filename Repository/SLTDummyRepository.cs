using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
namespace Saltr.UnitySdk.Repository
{
  internal class SLTDummyRepository : ISLTRepository
    {

        public SLTDummyRepository()
        {
        }

        public void CacheObject(string name, string version, object obj)
        {

        }

        public object GetObjectFromApplication(string fileName)
        {
			return MiniJSON.Json.Deserialize(Resources.Load<TextAsset>(fileName).text);
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
			return "";
        }

        public void SaveObject(string name, object obj)
        {
            
        }
    }
}
