using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saltr_unity_sdk;
using System.IO;
using UnityEngine;
namespace Assets
{
  public  class SLTDummyRepository : ISLTRepository
    {
        private DirectoryInfo _applicationDirectory;
        private FileStream _fileStream;

        public SLTDummyRepository()
        {
            _applicationDirectory = new DirectoryInfo(Application.dataPath);
            _fileStream = null;
        }

        public void cacheObject(string name, string version, object Object)
        {

        }

        public object getObjectFromApplication(string fileName)
        {
			return MiniJSON.Json.Deserialize(Resources.Load<TextAsset>(fileName).text);
        }

        public object getObjectFromCache(string fileName)
        {
			return null;
        }

        public object getObjectFromStorage(string name)
        {
			return null;
        }

        public string getObjectVersion(string name)
        {
			return "";
        }

        public void saveObject(string name, object Object)
        {
            
        }
    }
}
