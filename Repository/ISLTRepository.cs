using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Repository
{
    public interface ISLTRepository
    {
        string GetObjectVersion(string name);

        void SaveObjectInPersistentStorage<T>(string name, T objectToSave);

        T GetObjectFromPersistentStorage<T>(string name) where T : class;

        void CacheObject<T>(string name, T objectToSave, string version = null);

        T GetObjectFromCache<T>(string fileName) where T : class;

        T GetObjectFromApplication<T>(string fileName) where T : class;
    }
}
