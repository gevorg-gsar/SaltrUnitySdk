using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using GAFEditor.Utils;

namespace Saltr.UnitySdk.Repository
{
    public class SLTMobileRepository : ISLTRepository
    {
        #region Fields

        private FileStream _fileStream;
        private DirectoryInfo _storageDirectory;
        private DirectoryInfo _applicationDirectory;
        private DirectoryInfo _cacheDirectory;

        #endregion Fields

        #region Ctor

        public SLTMobileRepository()
        {
            _applicationDirectory = new DirectoryInfo(Application.persistentDataPath);
            _storageDirectory = new DirectoryInfo(Application.persistentDataPath);
            _cacheDirectory = new DirectoryInfo(Application.temporaryCachePath);

            Debug.Log("storageDirectory: " + _storageDirectory.FullName);
            Debug.Log("cacheDir: " + _cacheDirectory.FullName);
        }

        #endregion Ctor

        #region Business Methods

        public void CacheObject(string name, string version, object objectToSave)
        {
            string filePath = Path.Combine(_cacheDirectory.FullName, name);
            SaveObjectInFile(filePath, objectToSave);

            filePath = Path.Combine(_cacheDirectory.FullName, name.Replace(".", string.Empty) + "_VERSION_");
            SaveObjectInFile(filePath, new { _VERSION_ = version });
        }

        public object GetObjectFromApplication(string fileName)
        {
            //string file = _applicationDirectory + "/" + fileName;
            // return getIntenrnal(new FileInfo(file));
            TextAsset file = Resources.Load<TextAsset>(fileName);
            return file != null ? Json.Deserialize(Resources.Load<TextAsset>(fileName).text) : null;
        }

        public object GetObjectFromCache(string fileName)
        {
            string file = Path.Combine(_cacheDirectory.FullName, fileName);
            // if (File.Exists(file))
            return GetObjectFromFile(new FileInfo(file));
            //  else
            //   return null;
        }

        public object GetObjectFromStorage(string name)
        {
            //   string file = _storageDirectory + "/" + name;
            // return getIntenrnal(new FileInfo(file));
            Debug.Log(name);
            return Json.Deserialize(Resources.Load<TextAsset>(name).text);
        }

        public string GetObjectVersion(string name)
        {
            string file = Path.Combine(_cacheDirectory.FullName, (name.Replace(".", string.Empty) + "_VERSION_"));
            object obj = GetObjectFromFile(new FileInfo(file));
            Dictionary<string, object> dict = (Dictionary<string, object>)obj;

            if (dict != null && dict.ContainsKey("_VERSION_"))
            {
                return dict["_VERSION_"].ToString();
            }

            return string.Empty;
        }

        public void SaveObject(string name, object objectToSave)
        {
            string resolvedPath = Path.Combine(_storageDirectory.FullName, name);
            SaveObjectInFile(resolvedPath, objectToSave);
        }

        #endregion Business Methods

        #region Internal Methods

        private object GetObjectFromFile(FileInfo file)
        {
            if (file.Exists)
            {
                try
                {
                    string strObject = File.ReadAllText(file.FullName);

                    if (string.IsNullOrEmpty(strObject))
                    {
                        return Json.Deserialize(strObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("[MobileStorageEngine] : error while getting object.\nError : " + e.Message);
                }
            }
            return null;
        }

        private void SaveObjectInFile(string file, object objectToSave)
        {
            _fileStream = new FileStream(file, FileMode.Create);
            _fileStream.Close();

            File.WriteAllText(file, LitJson.JsonMapper.ToJson(objectToSave));
        }

        #endregion Internal Methods

    }
}
