using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.utils
{
    internal class HTTPStatus
    {
        public static readonly int HTTP_STATUS_400 = 400;
        public static readonly int HTTP_STATUS_403 = 403;
        public static readonly int HTTP_STATUS_404 = 404;
        public static readonly int HTTP_STATUS_500 = 500;
        public static readonly int HTTP_STATUS_502 = 502;
        public static readonly int HTTP_STATUS_503 = 503;
        public static readonly int HTTP_STATUS_OK = 200;
        public static readonly int HTTP_STATUS_NOT_MODIFIED = 304;
        public static readonly List<int> HTTP_ERROR_CODES = new List<int>(){ HTTP_STATUS_400, HTTP_STATUS_403, HTTP_STATUS_404,HTTP_STATUS_500,HTTP_STATUS_502, HTTP_STATUS_503};

        public static bool isInErrorCodes(int statusCode)
        {
            return HTTP_ERROR_CODES.Contains(statusCode);
        }

    }
}
