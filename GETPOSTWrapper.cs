using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets;
using System.Timers;

public class GETPOSTWrapper : MonoBehaviour
{
    private float elapsedTime = 0.0f;
    private bool isDownloading = false;
    private WWW WWW = null;
    private int dropTimeout;
    private SLTResource _resource;
	private Action<SLTResource> _appDataLoadSuccessHandler;
    private Action<SLTResource> _appDataLoadFailHandler;

    void Start() { }

    void Update()
    {
		if(isDownloading)
		{
        	elapsedTime += Time.deltaTime;
        	if (elapsedTime >= dropTimeout)
        	{
            	Debug.Log("[Asset] Loading is too long, so it stopped by force.");
				isDownloading = false;
				if(WWW!=null)
				{	
					WWW.Dispose();
					WWW=null;
				}
				_appDataLoadFailHandler(_resource);
        	} 
			else if(WWW!=null && WWW.isDone)
			{	
				//if (WWW == null)
				//{
				//	Debug.Log("WWW is null!");
				//	resource.ioErrorHandler();
				//	isDownloading = false;
				//}
				
				if (WWW.error == null)
				{
					Debug.Log("Download is finished!" + WWW.text);
					_resource.data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(WWW.text);
					isDownloading = false;
					WWW.Dispose();
					WWW=null;
					_appDataLoadSuccessHandler(_resource);
				}
				
				else
				{
					Debug.Log("Download error : " + WWW.error);
					isDownloading = false;
					WWW.Dispose();
					WWW=null;
					_appDataLoadFailHandler(_resource);
					//_resource.ioErrorHandler();
				}
			}
		} 
		else elapsedTime = 0;
	}
	
	public void GET(string url, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
    {
        if (isDownloading){
			Debug.Log ("Force Returned from GET");
            return;
		}
        this._appDataLoadFailHandler = appDataLoadFailHandler;
		this._appDataLoadSuccessHandler = appDataLoadSuccessHandler;
        this._resource = resource;

        if (resource.ticket.dropTimeout != 0)
            this.dropTimeout = resource.ticket.dropTimeout;
        else
            this.dropTimeout = 3;

        Debug.Log("URL " + url);
        StartDownloading(url);
    }



    public WWW POST(string url, Dictionary<string, string> post)
    {
        //WWWForm form = new WWWForm();
        //foreach (KeyValuePair<String, String> post_arg in post)
        //{
        //    form.AddField(post_arg.Key, post_arg.Value);
        //}
        //WWW www = new WWW(url, form);


        //StartCoroutine(WaitForRequest(www));
        //return www;

        return null;
    }

    private void StartDownloading(string url)
    {
        elapsedTime = 0.0f;
        isDownloading = true;
        WWW = new WWW(url);
    }
}
