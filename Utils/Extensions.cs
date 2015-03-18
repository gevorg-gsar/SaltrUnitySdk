using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Saltr.UnitySdk.Utils
{
    /// <summary>
    /// Some usefull extensions for parsing string->object dictionaries.
    /// </summary>
    public static class Extensions // TODO @gyln: add null-checks all over the place?
    {
        /// <summary>
        /// Gets the value corresponding to the given key.
        /// </summary>
        /// <returns>The corresponding value, if the key excists, otherwise <c>null</c>.</returns>
        public static object GetValue(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            } 
             
            return null;
        }

        /// <summary>
        /// Gets the value corresponding to the given key.
        /// </summary>
        /// <returns>The corresponding value, if the key excists, otherwise <c>null</c>.</returns>
        /// <typeparam name="Type">The value will be cast to this type.</typeparam>
        public static Type GetValue<Type>(this Dictionary<string, object> dictionary, string key) where Type : class
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key] as Type;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
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
