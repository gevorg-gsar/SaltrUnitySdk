using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Saltr.UnitySdk.Network
{
    public class SLTDownloadManager : MonoBehaviour, IDisposable
    {
        #region Constants

        private const string DownloadManagerGameObjectName = "SLTDownloadManager";

        #endregion  Constants

        #region Fields

        private bool isDownloading = false;
        private static SLTDownloadManager _instance;
        private Queue<SLTDownloadRequest> _downloadRequests = null;

        #endregion Fields

        #region Properties

        public static SLTDownloadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject downloadManagerGameObject = new GameObject(DownloadManagerGameObjectName);
                    _instance = downloadManagerGameObject.AddComponent<SLTDownloadManager>();
                    DontDestroyOnLoad(downloadManagerGameObject);
                }

                return _instance;
            }
        }

        #endregion Properties

        #region MonoBehaviour

        void Awake()
        {
            _downloadRequests = new Queue<SLTDownloadRequest>();
        }

        void FixedUpdate()
        {
            if (!isDownloading)
            {
                this.enabled = false;
            }

            Execute();
        }

        void Destroy()
        {
            this.Dispose();
            _instance = null;
        }

        #endregion  MonoBehaviour

        #region Public Methods

        public void AddDownload(SLTDownloadRequest downloadRequest)
        {
            _downloadRequests.Enqueue(downloadRequest);
            this.enabled = true;
        }

        public void AddDownload(string url, Action<SLTDownloadResult> downloadCallback, float timeout = 0)
        {
            AddDownload(new SLTDownloadRequest(url, downloadCallback) { Timeout = timeout });
        }

        public void Execute()
        {
            if (isDownloading)
            {
                return;
            }

            StartCoroutine(Download());
        }

        public void Dispose()
        {
            isDownloading = false;
            StopAllCoroutines();
            
            _downloadRequests.Clear();
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerator Download()
        {
            if (_downloadRequests.Count == 0)
            {
                yield return true;
            }
            this.isDownloading = true;
            this.enabled = true;

            while (_downloadRequests.Count > 0)
            {
                SLTDownloadRequest downloadRequest = _downloadRequests.Dequeue();
                
                downloadRequest.StartRequest();
                
                while (downloadRequest.WWW != null && !downloadRequest.WWW.isDone)
                {
                    downloadRequest.CheckTimeout(true);

                    yield return null;
                }

                //yield return downloadRequest.WWW;

                SLTDownloadResult downlaodResult = null;
                if (!downloadRequest.IsTimeout && downloadRequest.WWW != null)
                {
                    downlaodResult = new SLTDownloadResult(downloadRequest.WWW.error) 
                    { 
                        Text = downloadRequest.WWW.text ?? "",
                        Bytes = downloadRequest.WWW.bytes,
                        Texture = downloadRequest.WWW.texture,
                        StateObject = downloadRequest.StateObject 
                    };
                    downloadRequest.DownloadCallback(downlaodResult);
                }
                else
                {
                    downlaodResult = new SLTDownloadResult(string.Format("{0} - {1} - {2} - {3}", downloadRequest.Url, "Request timeout", downloadRequest.StartTime.ToString(), Time.time.ToString()));
                    downloadRequest.DownloadCallback(downlaodResult);
                }
            }

            this.isDownloading = false;
            this.enabled = false;
        }

        #endregion Private Methods

    }
}