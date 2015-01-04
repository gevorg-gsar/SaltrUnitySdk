using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.repository
{
   internal interface ISLTRepository
    {
        object GetObjectFromStorage(string name);

        object GetObjectFromCache(string fileName);

        string GetObjectVersion(string name);

		void SaveObject(string name, object objectToSave);

		void CacheObject(string name, string version, object objectToSave);

        object GetObjectFromApplication(string fileName);
    }
}
