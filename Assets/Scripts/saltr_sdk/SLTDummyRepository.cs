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

        private object getInternal(FileInfo file)
        {
            try
            {
                if (!File.Exists(file.FullName))
                    return null;

                string stringData = File.ReadAllText(file.FullName);

                if (stringData != null)
                    return null;
                else
                    return MiniJSON.Json.Deserialize(stringData);


            }
            catch (Exception e)
            {
                Debug.Log("[MobileStorageEngine] : error while getting object.\nError :  message : '" + e.Message);
                return null;
            }
        }

        public void cacheObject(string name, string version, object Object)
        {

        }

        public object getObjectFromApplication(string fileName)
        {
            throw new NotImplementedException();
        }

        public object getObjectFromCache(string fileName)
        {
            return "";
        }

        public object getObjectFromStorage(string name)
        {
            return "";
        }

        public string getObjectVersion(string name)
        {
            return "";
        }

        public void saveObject(string name, object Object)
        {
            throw new NotImplementedException();
        }
    }
}
