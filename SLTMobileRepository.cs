using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using saltr_unity_sdk;
using UnityEngine;

namespace Assets
{
    public class SLTMobileRepository : ISLTRepository
    {
        private DirectoryInfo _storageDirectory;
        private DirectoryInfo _applicationDirectory;
        private DirectoryInfo _cacheDirectory;
        private FileStream _fileStream;

        public SLTMobileRepository()
        {
            _applicationDirectory = new DirectoryInfo(Application.persistentDataPath);
            _storageDirectory = new DirectoryInfo(Application.persistentDataPath);
            _cacheDirectory = new DirectoryInfo(Application.temporaryCachePath);

            Debug.Log("storageDirectory: " + _storageDirectory.FullName);
            Debug.Log("cacheDir: " + _cacheDirectory.FullName);
        }

        public void cacheObject(string name, string version, object Object)
        {
            string filePath = _cacheDirectory + "/" + name;
            saveInternal(filePath, Object);
            filePath = _cacheDirectory + "/" + name.Replace(".", "") + "_VERSION_";

            saveInternal(filePath, new { _VERSION_ = version });
        }

        public object getObjectFromApplication(string fileName)
        {
            string file = _applicationDirectory + "/" + fileName;
            return getIntenrnal(new FileInfo(file));
        }

        public object getObjectFromCache(string fileName)
        {
            string file = _cacheDirectory + "/" + fileName;

            return getIntenrnal(new FileInfo(file));
        }

        public object getObjectFromStorage(string name)
        {
            string file = _storageDirectory + "/" + name;
            return getIntenrnal(new FileInfo(file));
        }

        public string getObjectVersion(string name)
        {
            string file = _cacheDirectory + "/" + name.Replace(".", "") + "_VERSION_";
            object obj = getIntenrnal(new FileInfo(file));
            Dictionary<string, object> dict = (Dictionary<string, object>)obj;
            if (dict == null)
                return "";

            if (dict.ContainsKey("_VERSION_"))
                return dict["_VERSION_"].ToString();
            else
                return "";
        }

        public void saveObject(string name, object Object)
        {
            string resolvedPath = _storageDirectory + "/" + name;
            saveInternal(resolvedPath, Object);
        }

        private object getIntenrnal(FileInfo file)
        {
            try
            {
                if (!file.Exists)
                    return null;

                string stringData = File.ReadAllText(file.FullName);


                if (stringData == null)
                    return null;
                else
                    return MiniJSON.Json.Deserialize(stringData);
            }
            catch (Exception e)
            {
                Debug.Log("[MobileStorageEngine] : error while getting object.\nError : " + e.Message);
                return null;
            }
        }

        private void saveInternal(string file, object objectToSave)
        {
            _fileStream = new FileStream(file, FileMode.Create);
            _fileStream.Close();

            File.WriteAllText(file, LitJson.JsonMapper.ToJson(objectToSave));
        }

    }
}
