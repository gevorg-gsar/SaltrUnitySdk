using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using saltr.resource;

namespace saltr.utils
{
	public class GETPOSTWrapper : MonoBehaviour
	{
	    private float elapsedTime = 0.0f;
	    private bool isDownloading = false;
	    private WWW WWW = null;
	    private int dropTimeout;
	    private SLTResource _resource;
	    private Action<SLTResource> _appDataLoadSuccessHandler;
	    private Action<SLTResource> _appDataLoadFailHandler;

		void Awake()
		{
			DontDestroyOnLoad (gameObject);
		}

	    void Start() { }

	    void Update()
	    {
	        if (isDownloading)
	        {
	            elapsedTime += Time.deltaTime;
	            if( elapsedTime >= dropTimeout)
	            {
	                Debug.Log("[Asset] Loading is too long, so it stopped by force.");
	                isDownloading = false;
	                if (WWW != null)
	                {
	                    WWW.Dispose();
	                    WWW = null;
	                }
	                _appDataLoadFailHandler(_resource);
	            }
	            else if (WWW != null && WWW.isDone)
	            {
	                //if (WWW == null)
	                //{
	                //	Debug.Log("WWW is null!");
	                //	resource.ioErrorHandler();
	                //	isDownloading = false;
	                //}

	                if (WWW.error == null)
	                {
	                    //Debug.Log("Download is finished!" + WWW.text);
	                    _resource.data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(WWW.text);
	                    isDownloading = false;
	                    WWW.Dispose();
	                    WWW = null;
	                    _appDataLoadSuccessHandler(_resource);
	                }

	                else
	                {
	                    Debug.Log("Download error : " + WWW.error);
	                    isDownloading = false;
	                    WWW.Dispose();
	                    WWW = null;
	                    _appDataLoadFailHandler(_resource);
	                    //_resource.ioErrorHandler();
	                }
	            }
	        }
	        else elapsedTime = 0;
	    }

	    public void GET(string url, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
	    {
	        if (isDownloading)
	        {
	            Debug.Log("Force Returned from GET");
	            return;
	        }
	        this._appDataLoadFailHandler = appDataLoadFailHandler;
	        this._appDataLoadSuccessHandler = appDataLoadSuccessHandler;
	        this._resource = resource;

	        if (resource.ticket.dropTimeout != 0)
	            this.dropTimeout = resource.ticket.dropTimeout;
	        else
	            this.dropTimeout = 3;

	        StartDownloading(url);
	    }



	    public WWW POST(string url, Dictionary<string, string> post, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
	    {
	        Debug.Log("EnteredToPost" + "url: " +  url);

	        WWWForm form = new WWWForm();

	        this._appDataLoadFailHandler = appDataLoadFailHandler;
	        this._appDataLoadSuccessHandler = appDataLoadSuccessHandler;
	        this._resource = resource;

	        if (resource.ticket.dropTimeout != 0)
	            this.dropTimeout = resource.ticket.dropTimeout;
	        else
	            this.dropTimeout = 3;

	      
	            form.AddField("args", MiniJSON.Json.Serialize( post));
	            form.AddField("cmd", SLTConfig.ACTION_DEV_SYNC_FEATURES);


	            //foreach (var item in post.Keys)
	            //{
	            //    post[item] = MiniJSON.Json.Serialize(post[item]);
	            //}


	            Debug.Log(MiniJSON.Json.Serialize(post));
	      


	        //foreach (KeyValuePair<String, String> post_arg in post)
	        //{
	        //    form.AddField(post_arg.Key, post_arg.Value);
	        //}

	        isDownloading = true;
	        elapsedTime = 0f;
	         WWW = new WWW(url, form);


	        return null;
	    }

	    private void StartDownloading(string url)
	    {
	        elapsedTime = 0.0f;
	        isDownloading = true;
	        WWW = new WWW(url);
	    }
	}
}
