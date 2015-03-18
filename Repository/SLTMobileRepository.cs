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
    public class SLTMobileRepository : ISLTRepository
    {
        #region Fields

        private DirectoryInfo _cacheDirectory;
        private DirectoryInfo _storageDirectory;

        #endregion Fields

        #region Ctor

        public SLTMobileRepository()
        {
            _cacheDirectory = new DirectoryInfo(Application.temporaryCachePath);
            _storageDirectory = new DirectoryInfo(Application.persistentDataPath);

            Debug.Log("cacheDir: " + _cacheDirectory.FullName);
            Debug.Log("storageDirectory: " + _storageDirectory.FullName);
        }

        #endregion Ctor

        #region Public Methods

        public void SaveObject<T>(string name, T objectToSave)
        {
            string filePath = Path.Combine(_storageDirectory.FullName, name);
            SaveObjectInFile<T>(filePath, objectToSave);
        }

        public void CacheObject<T>(string name, T objectToSave, string version = null)
        {
            string filePath = Path.Combine(_cacheDirectory.FullName, name);
            SaveObjectInFile<T>(filePath, objectToSave);

            if (string.IsNullOrEmpty(version))
            {
                filePath = Path.Combine(_cacheDirectory.FullName, name.Replace(".", string.Empty) + "_VERSION_");
                SaveObjectInFile<object>(filePath, new { _VERSION_ = version });
            }
        }

        public T GetObjectFromApplication<T>(string fileName) where T : class
        {
            TextAsset file = Resources.Load<TextAsset>(fileName);
            return file != null ? JsonConvert.DeserializeObject<T>(Resources.Load<TextAsset>(fileName).text) : null;
        }

        public T GetObjectFromCache<T>(string fileName) where T : class
        {
            string filePath = Path.Combine(_cacheDirectory.FullName, fileName);
            return GetObjectFromFile<T>(filePath);
        }

        public T GetObjectFromStorage<T>(string name) where T : class
        {
            //@TODO: Gor it seems Path.Combine method call is missing here.
            Debug.Log(name);
            return JsonConvert.DeserializeObject<T>(Resources.Load<TextAsset>(name).text);
        }

        public string GetObjectVersion(string name)
        {
            string filePath = Path.Combine(_cacheDirectory.FullName, (name.Replace(".", string.Empty) + "_VERSION_"));
            object obj = GetObjectFromFile<object>(filePath);
            Dictionary<string, object> dict = (Dictionary<string, object>)obj;

            if (dict != null && dict.ContainsKey("_VERSION_"))
            {
                return dict["_VERSION_"].ToString();
            }

            return string.Empty;
        }

        #endregion Public Methods

        #region Internal Methods

        private T GetObjectFromFile<T>(string filePath) where T : class
        {
            FileInfo file = new FileInfo(filePath);

            if (file.Exists)
            {
                try
                {
                    string strObject = File.ReadAllText(file.FullName);

                    if (!string.IsNullOrEmpty(strObject))
                    {
                        return JsonConvert.DeserializeObject<T>(strObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("[Caching] : error while getting object from a file.\nError : " + e.Message);
                }
            }

            return null;
        }

        private void SaveObjectInFile<T>(string filePath, T objectToSave)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                }
                File.WriteAllText(filePath, JsonConvert.SerializeObject(objectToSave));
            }
            catch (Exception e)
            {
                Debug.Log("[Caching] : error while saving object in a file.\nError : " + e.Message);
            }
        }

        #endregion Internal Methods

    }
}
