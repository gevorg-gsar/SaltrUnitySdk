﻿using System;
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
        /// Tries to convert the the object to an integer. If succesfull returns it otherwise returns the specified value.
        /// </summary>
        public static int ToIntegerOr(this object obj, int value)
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
        public static int ToIntegerOrZero(this object obj)
        {
            return obj.ToIntegerOr(0);
        }

        internal static void RemoveEmptyOrNull(this Dictionary<string, object> dictionary)
        {
            for (int i = dictionary.Keys.Count - 1; i >= 0; --i)
            {
                string key = dictionary.Keys.ElementAt(i);
                object value = dictionary[key];
                if (value == null || (value as String == String.Empty))
                {
                    dictionary.Remove(key);
                }
                else if (value is Dictionary<string, object>)
                {
                    RemoveEmptyOrNull(value as Dictionary<string, object>);
                }
            }
        }

    }
}
