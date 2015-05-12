
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
        #region Properties

        public string Url { get; set; }

        public object StateObject { get; set; }

        public float StartTime { get; private set; }

        public bool IsTimeout { get; private set; }
        
        public float Timeout { get; set; }

        public Action<SLTDownloadResult> DownloadCallback { get; set; }

        public byte[] Bytes { get; set; }

        public WWWForm Form { get; set; }

        public Dictionary<string, string> Header { get; set; }

        public WWW WWW { get; private set; }

        public float Progress
        {
            get
            {
                if (WWW == null)
                {
                    return 0f;
                }
                else
                {
                    return WWW.progress;
                }
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

        public void StartRequest()
        {
            StartTime = Time.time;

            if (Header != null)
            {
                this.WWW = new WWW(Url, Bytes, Header);
            }
            else if (Bytes != null)
            {
                this.WWW = new WWW(Url, Bytes);
            }
            else if (Form != null)
            {
                this.WWW = new WWW(Url, Form);
            }
            else
            {
                this.WWW = new WWW(Url);
            }
        }

        public bool CheckTimeout(bool dispose)
        {
            IsTimeout = false;
            if (Timeout > 0)
            {
                float now = Time.time;
                if ((now - StartTime) > Timeout)
                {
                    IsTimeout = true;
                }

                if (dispose && IsTimeout)
                {
                    WWW tmpWWW = WWW;
                    WWW = null;

                    tmpWWW.Dispose(); //@GORTODO: check if "Download" method returns from yield return.
                }
            }

            return IsTimeout;
        }

        #endregion  Public Methods

    }
}
