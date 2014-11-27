using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace saltr.utils
{
    public static class Utils
    {
        public static string formatString(string format, params string[] args)
        {
		
            for (int i = 0; i < args.Count(); i++)
            {
                Regex rgx = new Regex("{" + i + "}");
              format = rgx.Replace(format, args[i]);
            }
			//Debug.Log(format);        
			return format;
        }

//		public static function checkEmailValidation(email:String):Boolean {
//			var emailExpression:RegExp = /([a-z0-9._-]+?)@([a-z0-9.-]+)\.([a-z]{2,4})/;
//			return emailExpression.test(email);
//		}

		public static bool validEmail(string email)
		{
			string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" 
				+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" 
					+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
			
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			return regex.IsMatch(email);
		}

    }
}
