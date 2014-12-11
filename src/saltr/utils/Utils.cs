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

		public static string getHumanReadableDeviceModel(string deviceModel)
		{
			switch(deviceModel)
			{
			case "iPod1,1":
				return "iPod Touch";
			case "iPod2,1":
				return "iPod Touch"; // Second Generation
			case "iPod3,1":
				return "iPod Touch"; // Third Generation
			case "iPod4,1":
				return "iPod Touch"; // Fourth Generation
			case "iPhone1,1":
				return "iPhone";
			case "iPhone1,2":
				return "iPhone 3G";
			case "iPhone2,1":
				return "iPhone 3GS";
			case "iPad1,1":
				return "iPad";
			case "iPad2,1":
				return "iPad 2";
			case "iPad3,1":
				return "iPad 3"; // "3rd Generation iPad";
			case "iPhone3,1":
				return "iPhone 4";
			case "iPhone4,1":
				return "iPhone 4S";
			case "iPhone5,1":
				return "iPhone 5"; // (model A1428, AT&T/Canada)
			case "iPhone5,2":
				return "iPhone 5"; // (model A1429, everything else)
			case "iPad3,4":
				return "iPad 4"; // "4th Generation iPad"; ipad retina?
			case "iPad2,5":
				return "iPad Mini";
			case "iPhone5,3":
				return "iPhone 5c"; // (model A1456, A1532 | GSM)
			case "iPhone5,4":
				return "iPhone 5c"; // (model A1507, A1516, A1526 (China), A1529 | Global)
			case "iPhone6,1":
				return "iPhone 5s"; // (model A1433, A1533 | GSM)
			case "iPhone6,2":
				return "iPhone 5s"; // (model A1457, A1518, A1528 (China), A1530 | Global)
			case "iPad4,1":
				return "iPad Air"; // 5th Generation - Wifi
			case "iPad4,2":
				return "iPad Air"; // 5th Generation - Cellular
			case "iPad4,4":
				return "iPad Mini 2"; // 2nd Generation - Wifi
			case "iPad4,5":
				return "iPad Mini 2"; // 2nd Generation - Cellular
			case "iPhone7,1":
				return "iPhone 6 Plus";
			case "iPhone7,2":
				return "iPhone 6";
			default:
				return "Unrecognised(" + deviceModel + ")";
			}
		}
    }
}
