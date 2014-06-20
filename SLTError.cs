using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTStatus
    {
        public static readonly int AUTHORIZATION_ERROR = 1001;
        public static readonly int VALIDATION_ERROR = 1002;
        public static readonly int API_ERROR = 1003;

        public static readonly int GENERAL_ERROR_CODE = 2001;
        public static readonly int CLIENT_ERROR_CODE = 2002;

        public static readonly int CLIENT_FEATURES_PARSE_ERROR = 2050;
        public static readonly int CLIENT_EXPERIMENTS_PARSE_ERROR = 2051;
        public static readonly int CLIENT_LEVELS_PARSE_ERROR = 2052;

        private int _statusCode;

        public int statusCode
        {
            get { return _statusCode; }
        }
        private string _statusMessage;

        public string statusMessage
        {
            get { return _statusMessage; }
        }

        public SLTStatus(int code, string message)
        {
            _statusCode = code;
            _statusMessage = message;
        }

    }
}
