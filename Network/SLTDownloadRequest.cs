
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Saltr.UnitySdk.Network
{
    public class SLTDownloadRequest
    {
        #region Constants

        private const string DownloadResultFormat = @"{0} - {1}";

        #endregion Constants

        #region Properties

        public string Url { get; set; }

        public object StateObject { get; set; }

        public SLTTimeOut TimeOut { get; set; }

        public Action<SLTDownloadResult> DownloadCallback { get; set; }

        public byte[] Bytes { get; set; }

        public WWWForm Form { get; set; }

        public Dictionary<string, string> Header { get; set; }

        public WWW WWW { get; private set; }

        public float Progress
        {
            get
            {
                if (WWW == null) { return 0f; }
                return WWW.progress;
            }
        }

        #endregion  Properties

        #region Ctor

        public SLTDownloadRequest(string url, Action<SLTDownloadResult> downloadCallback)
        {
            this.Url = url;
            this.DownloadCallback = downloadCallback;
        }

        public SLTDownloadRequest(string url, Action<SLTDownloadResult> downloadCallback, WWWForm form)
            : this(url, downloadCallback)
        {
            this.Form = form;
        }

        public SLTDownloadRequest(string url, Action<SLTDownloadResult> downloadCallback, byte[] bytes)
            : this(url, downloadCallback)
        {
            this.Bytes = bytes;
        }

        public SLTDownloadRequest(string url, Action<SLTDownloadResult> downloadCallback, byte[] bytes, Dictionary<string, string> header)
            : this(url, downloadCallback, bytes)
        {
            this.Header = header;
        }

        #endregion Ctor

        #region Public Methods

        public WWW CreateRequest()
        {
            if (Header != null)
            {
                return new WWW(Url, Bytes, Header);
            }
            if (Bytes != null)
            {
                return new WWW(Url, Bytes);
            }
            if (Form != null)
            {
                return new WWW(Url, Form);
            }

            this.WWW = new WWW(Url);

            return this.WWW;
        }

        public bool CheckTimeout(bool notify)
        {
            bool isTimeout = false;
            if (TimeOut != null)
            {
                isTimeout = TimeOut.CheckTimeout(Progress);

                if (notify && isTimeout && DownloadCallback != null)
                {
                    DownloadCallback(new SLTDownloadResult(string.Format(DownloadResultFormat, Url, "Request timeout")));
                    WWW.Dispose(); //@GORTODO: check if "Download" method returns from yield return.
                    //WWW = null;
                }
            }

            return isTimeout;
        }

        #endregion  Public Methods

    }
}
