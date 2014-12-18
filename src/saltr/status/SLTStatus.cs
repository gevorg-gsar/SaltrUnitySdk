using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	/// <summary>
	/// Represents a status of various operations carried out by the SDK.
	/// </summary>
    public class SLTStatus
    {
		/// <summary>
		/// Integer status codes.
		/// </summary>
		public enum Code
		{
        	AUTHORIZATION_ERROR = 1001,
			VALIDATION_ERROR = 1002,
			API_ERROR = 1003,
			PARSE_ERROR = 1004,

			REGISTRATION_REQUIRED = 2001,
			CLIENT_ERROR = 2002,

			CLIENT_APP_DATA_LOAD_FAIL = 2040,
			CLIENT_LEVEL_CONTENT_LOAD_FAIL = 2041,
			CLIENT_APP_DATA_CONCURRENT_LOAD_REFUSED = 2042,


			CLIENT_FEATURES_PARSE_ERROR = 2050,
			CLIENT_EXPERIMENTS_PARSE_ERROR = 2051,
			CLIENT_LEVELS_PARSE_ERROR = 2052,

			UNKNOWN_ERROR = -1
		}

		private Code _statusCode;

		/// <summary>
		/// Gets and integer status code.
		/// </summary>
		public Code statusCode
        {
            get { return _statusCode; }
        }

        private string _statusMessage;
		/// <summary>
		/// Gets a human-readable status message.
		/// </summary>
        public string statusMessage
        {
            get { return _statusMessage; }
        }

		internal SLTStatus(int code, string message)
        {
			if(Enum.IsDefined(typeof(Code), code))
				_statusCode = (Code)code;
			else
				_statusCode = Code.UNKNOWN_ERROR;

            _statusMessage = message;
			UnityEngine.Debug.Log(message);
        }

		internal SLTStatus(Code code, string message)
		{
			_statusCode = code;
			_statusMessage = message;
			UnityEngine.Debug.Log(message);
		}

    }
}
