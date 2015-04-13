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
        private SLTDownloadRequest _currentDownloadRequest = null;
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

        #region Messages

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

            if (_currentDownloadRequest != null)
            {
                _currentDownloadRequest.CheckTimeout(true);
            }

            Execute();

        }

        void Destroy()
        {
            this.Dispose();
            _instance = null;
        }

        #endregion Messages

        #region Public Methods

        public void AddDownload(SLTDownloadRequest downloadRequest)
        {
            _downloadRequests.Enqueue(downloadRequest);
            this.enabled = true;
        }

        public void AddDownload(string url, Action<SLTDownloadResult> downloadCallback)
        {
            AddDownload(new SLTDownloadRequest(url, downloadCallback));
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
            if (_currentDownloadRequest != null && _currentDownloadRequest.WWW != null)
            {
                _currentDownloadRequest.WWW.Dispose();
            }
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
                downloadRequest.CreateRequest();
                _currentDownloadRequest = downloadRequest;

                yield return _currentDownloadRequest.WWW;

                downloadRequest.DownloadCallback(new SLTDownloadResult(downloadRequest.WWW) { StateObject = downloadRequest.StateObject });
            }

            this.isDownloading = false;
            this.enabled = false;
        }

        #endregion Private Methods

    }
}