using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Repository
{
    public interface ISLTRepository
    {
        string GetObjectVersion(string name);

        T GetObjectFromStorage<T>(string name) where T : class;

        T GetObjectFromCache<T>(string fileName) where T : class;

        void SaveObject<T>(string name, T objectToSave);

        void CacheObject<T>(string name, T objectToSave, string version = null);

        T GetObjectFromApplication<T>(string fileName) where T : class;
    }
}
