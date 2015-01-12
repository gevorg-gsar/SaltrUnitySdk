using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
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
        	AuthorizationError = 1001,
			ValidationError = 1002,
			APIError = 1003,
			ParseError = 1004,

			RegistrationRequired = 2001,
			ClientError = 2002,

			ClientAppDataLoadFail = 2040,
			ClientLevelContentLoadFail = 2041,
			ClientAppDataConcurrentLoadRefused = 2042,


			ClientFeaturesParseError = 2050,
			ClientExperimentsParseError = 2051,
			ClientLevelsParseError = 2052,

			UnknownError = -1
		}

		private Code _statusCode;

		/// <summary>
		/// Gets and integer status code.
		/// </summary>
		public Code StatusCode
        {
            get { return _statusCode; }
        }

        private string _statusMessage;
		/// <summary>
		/// Gets a human-readable status message.
		/// </summary>
        public string StatusMessage
        {
            get { return _statusMessage; }
        }

		internal SLTStatus(int code, string message)
        {
			if(Enum.IsDefined(typeof(Code), code))
				_statusCode = (Code)code;
			else
				_statusCode = Code.UnknownError;

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
