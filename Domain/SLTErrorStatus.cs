using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Saltr.UnitySdk.Domain
{
    /// <summary>
    /// Represents a status of various operations carried out by the SDK.
    /// </summary>
    public class SLTErrorStatus
    {
        #region Properties

        public string Message { get; set; }

        public SLTErrorStatusCode Code { get; set; }

        #endregion 
    }

    /// <summary>
    /// Integer status codes.
    /// </summary>
    public enum SLTErrorStatusCode
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
