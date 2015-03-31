using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
//using GAFEditor.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Saltr.UnitySdk.Game;

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

            if (!string.IsNullOrEmpty(version))
            {
                filePath = Path.Combine(_cacheDirectory.FullName, name.Replace(".", string.Empty) + "_VERSION_");
                SaveObjectInFile<SaltrVersion>(filePath, new SaltrVersion { Version = version });
            }
        }

        public T GetObjectFromApplication<T>(string fileName) where T : class
        {
            TextAsset file = Resources.Load<TextAsset>(fileName);
            return file != null ? JsonConvert.DeserializeObject<T>(Resources.Load<TextAsset>(fileName).text, new BoardConverter(), new SLTAssetTypeConverter()) : null;
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
            return JsonConvert.DeserializeObject<T>(Resources.Load<TextAsset>(name).text, new BoardConverter(), new SLTAssetTypeConverter());
        }

        public string GetObjectVersion(string name)
        {
            string filePath = Path.Combine(_cacheDirectory.FullName, (name.Replace(".", string.Empty) + "_VERSION_"));
            SaltrVersion saltrVersion = GetObjectFromFile<SaltrVersion>(filePath);
        
            return saltrVersion !=null ? saltrVersion.Version : null;
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
                        return JsonConvert.DeserializeObject<T>(strObject, new BoardConverter(), new SLTAssetTypeConverter());
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("[Caching] : error while getting object from a file.\nError : " + e.Message);
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

                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesExceptDictionaryKeysContractResolver()
                };

                File.WriteAllText(filePath, JsonConvert.SerializeObject(objectToSave, Formatting.None, settings));
            }
            catch (Exception e)
            {
                Debug.Log("[Caching] : error while saving object in a file.\nError : " + e.Message);
            }
        }

        #endregion Internal Methods

    }

    public class SaltrVersion
    {
        [JsonProperty("_VERSION_")]
        public string Version { get; set; }
    }
}
