using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace Saltr.UnitySdk.Repository
{
    internal class SLTMobileRepository : ISLTRepository
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

		public void CacheObject(string name, string version, object objectToSave)
        {
            string filePath = _cacheDirectory + "/" + name;
            saveInternal(filePath, objectToSave);
            filePath = _cacheDirectory + "/" + name.Replace(".", "") + "_VERSION_";

            saveInternal(filePath, new { _VERSION_ = version });
        }

        public object GetObjectFromApplication(string fileName)
        {
            //string file = _applicationDirectory + "/" + fileName;
            // return getIntenrnal(new FileInfo(file));
			TextAsset file = Resources.Load<TextAsset>(fileName);
			return file != null ? MiniJSON.Json.Deserialize(Resources.Load<TextAsset>(fileName).text) : null;
        }

        public object GetObjectFromCache(string fileName)
        {
            string file = _cacheDirectory + "/" + fileName;
           // if (File.Exists(file))
                return getIntenrnal(new FileInfo(file));
          //  else
             //   return null;
        }

        public object GetObjectFromStorage(string name)
        {
         //   string file = _storageDirectory + "/" + name;
           // return getIntenrnal(new FileInfo(file));
            Debug.Log(name);
            return MiniJSON.Json.Deserialize(Resources.Load<TextAsset>(name).text);
        }

        public string GetObjectVersion(string name)
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

		public void SaveObject(string name, object objectToSave)
        {
            string resolvedPath = _storageDirectory + "/" + name;
            saveInternal(resolvedPath, objectToSave);
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
