using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Saltr.UnitySdk.Utils
{
    public static class Extensions
    {
        public static object GetValue(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            } 
             
            return null;
        }

        public static Type GetValue<Type>(this Dictionary<string, object> dictionary, string key) where Type : class
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key] as Type;
            }

            return null;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

    }
}
