using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
   public interface ISLTRepository
    {
        object getObjectFromStorage(string name);

        object getObjectFromCache(string fileName);

        string getObjectVersion(string name);

        void saveObject(string name, object Object);

        void cacheObject(string name, string version, object Object);

        object getObjectFromApplication(string fileName);
    }
}
