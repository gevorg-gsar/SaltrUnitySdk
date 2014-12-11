using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.utils
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


        public static object getValue(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
                return null;
        }

		public static Type getValue<Type>(this Dictionary<string, object> dictionary, string key) where Type : class
		{
			if (dictionary.ContainsKey(key))
			{
				return dictionary[key] as Type;
			}
			else
				return null;
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
		
		public static float toFloatOr(this object obj, float value)
		{
			float x = value;
			string str = "";
			
			try
			{
				str = obj.ToString();
				
				x = float.Parse(str);
				return x;
			}
			
			catch
			{
				return x;
			}
		}

        public static float toFloatOrZero(this object obj)
        {
			return obj.toFloatOr(0);
        }


		public static int toIntegerOr(this object obj, int value)
		{
			int x = value;
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

        public static int toIntegerOrZero(this object obj)
        {
			return obj.toIntegerOr(0);
        }

    }
}
