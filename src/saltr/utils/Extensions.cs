using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.utils
{
	/// <summary>
	/// Some usefull extensions for parsing string->object dictionaries.
	/// </summary>
    public static class Extensions // TODO @gyln: add null-checks all over the place?
    {
		/// <summary>
		/// Tries to convert the the object to string->object dictionary. If succesfull returns it otherwise returns null.
		/// </summary>
		/// <returns>The dictionary or null.</returns>
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
	
		/// <summary>
		/// Gets the value corresponding to the given key.
		/// </summary>
		/// <returns>The corresponding value, if the key excists, otherwise <c>null</c>.</returns>
        public static object getValue(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
                return null;
        }

		/// <summary>
		/// Gets the value corresponding to the given key.
		/// </summary>
		/// <returns>The corresponding value, if the key excists, otherwise <c>null</c>.</returns>
		/// <typeparam name="Type">The value will be cast to this type.</typeparam>
		public static Type getValue<Type>(this Dictionary<string, object> dictionary, string key) where Type : class
		{
			if (dictionary.ContainsKey(key))
			{
				return dictionary[key] as Type;
			}
			else
				return null;
		}

		/// <summary>
		/// Tries to convert the the object to string->string dictionary. If succesfull returns it otherwise returns null.
		/// </summary>
		/// <returns>The dictionary or null.</returns>
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
	
		/// <summary>
		/// Tries to convert the the object to a float. If succesfull returns it otherwise returns the specified value.
		/// </summary>
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

		/// <summary>
		/// Tries to convert the the object to a float. If succesfull returns it otherwise returns zero.
		/// </summary>
        public static float toFloatOrZero(this object obj)
        {
			return obj.toFloatOr(0);
        }


		/// <summary>
		/// Tries to convert the the object to an integer. If succesfull returns it otherwise returns the specified value.
		/// </summary>
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

		/// <summary>
		/// Tries to convert the the object to an integer. If succesfull returns it otherwise returns zero.
		/// </summary>
        public static int toIntegerOrZero(this object obj)
        {
			return obj.toIntegerOr(0);
        }

    }
}
