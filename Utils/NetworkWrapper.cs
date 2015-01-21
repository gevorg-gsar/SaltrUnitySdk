using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using Saltr.UnitySdk.Resource;

namespace Saltr.UnitySdk.Utils
{
	/// <summary>
	/// Wrapper component over UnityEngine._www, used for network interactions in SDK.
	/// </summary>
	public class NetworkWrapper : MonoBehaviour
    {
        #region Fields

        private int _dropTimeout;
        private float _elapsedTime = 0.0f;
        private bool _isDownloading = false;
        private WWW _www = null;
        private SLTResource _resource;
        private Action<SLTResource> _appDataLoadSuccessHandler;
        private Action<SLTResource> _appDataLoadFailHandler;

        #endregion Fields

        #region Properties

        public bool IsBusy
        {
            get { return _isDownloading; }
        }

        #endregion Properties

        #region Messages

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start() { }

        private void Update()
        {
            if (_isDownloading)
            {
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime >= _dropTimeout)
                {
                    Debug.Log("[Asset] Loading is too long, so it stopped by force.");
                    _isDownloading = false;
                    if (_www != null)
                    {
                        _www.Dispose();
                        _www = null;
                    }
                    _appDataLoadFailHandler(_resource);
                }
                else if (_www != null && _www.isDone)
                {
                    //if (_www == null)
                    //{
                    //	Debug.Log("_www is null!");
                    //	resource.ioErrorHandler();
                    //	_isDownloading = false;
                    //}

                    if (_www.error == null)
                    {
                        //Debug.Log("Download is finished!" + _www.text);
                        _resource.Data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_www.text);
                        _isDownloading = false;
                        _www.Dispose();
                        _www = null;
                        _appDataLoadSuccessHandler(_resource);
                    }

                    else
                    {
                        Debug.Log("Download error : " + _www.error);
                        _isDownloading = false;
                        _www.Dispose();
                        _www = null;
                        _appDataLoadFailHandler(_resource);
                        //_resource.ioErrorHandler();
                    }
                }
            }
            else _elapsedTime = 0;
        }

        #endregion Messages

        #region Business Methods

        public void GET(string url, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
        {
            if (_isDownloading)
            {
                foreach (NetworkWrapper GPWrap in gameObject.GetComponents<NetworkWrapper>())
                {
                    if (!GPWrap.IsBusy)
                    {
                        GPWrap.GET(url, appDataLoadSuccessHandler, appDataLoadFailHandler, resource);
                        return;
                    }
                }
                gameObject.AddComponent<NetworkWrapper>().GET(url, appDataLoadSuccessHandler, appDataLoadFailHandler, resource);
                return;
            }
            this._appDataLoadFailHandler = appDataLoadFailHandler;
            this._appDataLoadSuccessHandler = appDataLoadSuccessHandler;
            this._resource = resource;

            if (resource.Ticket.DropTimeout != 0)
                this._dropTimeout = resource.Ticket.DropTimeout;
            else
                this._dropTimeout = 3;

            //			Debug.Log(url);
            StartDownloading(url);
        }

        public WWW POST(string url, Dictionary<string, string> post, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
        {
            Debug.Log("EnteredToPost" + "url: " + url);

            WWWForm form = new WWWForm();

            this._appDataLoadFailHandler = appDataLoadFailHandler;
            this._appDataLoadSuccessHandler = appDataLoadSuccessHandler;
            this._resource = resource;

            if (resource.Ticket.DropTimeout != 0)
            {
                this._dropTimeout = resource.Ticket.DropTimeout;
            }
            else
            { 
                this._dropTimeout = 3;
            }

            form.AddField("args", MiniJSON.Json.Serialize(post));
            form.AddField("cmd", SLTConstants.ActionDevSyncData);


            //foreach (var item in post.Keys)
            //{
            //    post[item] = MiniJSON.Json.Serialize(post[item]);
            //}


            Debug.Log(MiniJSON.Json.Serialize(post));



            //foreach (KeyValuePair<String, String> post_arg in post)
            //{
            //    form.AddField(post_arg.Key, post_arg.Value);
            //}

            _isDownloading = true;
            _elapsedTime = 0f;
            _www = new WWW(url, form);

            return null;
        }

        #endregion Business Methods

        #region Internal Mehods

        private void StartDownloading(string url)
        {
            _elapsedTime = 0.0f;
            _isDownloading = true;
            _www = new WWW(url);
        }

        #endregion Internal Mehods
        
	}
}
