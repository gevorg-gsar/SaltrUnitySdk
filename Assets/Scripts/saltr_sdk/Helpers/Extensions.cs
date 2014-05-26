using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public static class Extensions
    {
        public static Dictionary<string, object> toDictionaryOrNull(this object obj)
        {
            Dictionary<string, object> dictionaryToReturn = new Dictionary<string, object>();
            try
            {
                dictionaryToReturn = (Dictionary<string, object>)obj;

                return dictionaryToReturn;
            }

            catch
            {
                return null;
            }
        }

        public static Dictionary<string, string> toDictionaryStringOrNull(this object obj)
        {
            Dictionary<string, string> dictionaryToReturn = new Dictionary<string, string>();
            try
            {
                dictionaryToReturn = (Dictionary<string, string>)obj;

                return dictionaryToReturn;
            }

            catch
            {
                return null;
            }
        }


        public static int toIntegerOrZero( this object obj)
        {
            int x = 0;
            string str = "";

            try
            {
                str = obj.ToString();

                x = Int32.Parse(str);
                return x;
            }

            catch
            {
                return x;
            }
        }

    }
}
