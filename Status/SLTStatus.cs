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
        #region Fields

        private string _statusMessage;
        private SLTStatusCode _statusCode;

        #endregion Fields

        #region Properties

        public SLTStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// Gets a human-readable status message.
        /// </summary>
        public string StatusMessage
        {
            get { return _statusMessage; }
        }

        #endregion

        #region Ctor

        public SLTStatus(int statusCode, string statusMessage)
        {
            if (Enum.IsDefined(typeof(SLTStatusCode), statusCode))
            {
                _statusCode = (SLTStatusCode)statusCode;
            }
            else
            { 
                _statusCode = SLTStatusCode.UnknownError;
            }

            _statusMessage = statusMessage;
            UnityEngine.Debug.Log(statusMessage);
        }

        public SLTStatus(SLTStatusCode statusCode, string statusMessage)
        {
            _statusCode = statusCode;
            _statusMessage = statusMessage;
            UnityEngine.Debug.Log(statusMessage);
        }

        #endregion Ctor
        
    }

    /// <summary>
    /// Integer status codes.
    /// </summary>
    public enum SLTStatusCode
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
}
