﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTStatus
    {
        public const int AUTHORIZATION_ERROR = 1001;
		public const int VALIDATION_ERROR = 1002;
		public const int API_ERROR = 1003;

		public const int GENERAL_ERROR_CODE = 2001;
		public const int CLIENT_ERROR_CODE = 2002;

		public const int CLIENT_APP_DATA_LOAD_FAIL = 2040;
		public const int CLIENT_LEVEL_CONTENT_LOAD_FAIL = 2041;
		public const int CLIENT_FEATURES_PARSE_ERROR = 2050;
		public const int CLIENT_EXPERIMENTS_PARSE_ERROR = 2051;
		public const int CLIENT_LEVELS_PARSE_ERROR = 2052;

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