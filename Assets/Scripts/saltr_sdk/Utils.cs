using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace saltr_unity_sdk
{
    public static class Utils
    {
        public static string formatString(string format, params string[] args)
        {
            for (int i = 0; i < args.Count(); i++)
            {
                Regex rgx = new Regex("\\{" + i + "\\}");
                rgx.Replace(format, args[i]);
            }

            return format;
        }
    }
}
