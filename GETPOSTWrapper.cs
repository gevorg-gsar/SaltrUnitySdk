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
    private Action<SLTResource> _appDataLoadFailHandler;

    void Start() { }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= dropTimeout && isDownloading)
        {
            StopCoroutine("WaitForRequest");
            Debug.Log("[Asset] Loading is too long, so it stopped by force.");
            _appDataLoadFailHandler(_resource);
            isDownloading = false;
          //  WWW.Dispose();
        }
    }

    public void GET(string url, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
    {
        if (isDownloading)
            return;

        this._appDataLoadFailHandler = appDataLoadFailHandler;

        this._resource = resource;

        if (resource.ticket.dropTimeout != 0)
            this.dropTimeout = resource.ticket.dropTimeout;
        else
            this.dropTimeout = 3;

        Debug.Log("URL " + url);
        StartCoroutine(WaitForRequest(url, appDataLoadSuccessHandler, appDataLoadFailHandler, resource));
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

    private IEnumerator WaitForRequest(string url, Action<SLTResource> appDataLoadSuccessHandler, Action<SLTResource> appDataLoadFailHandler, SLTResource resource)
    {
        elapsedTime = 0.0f;
        isDownloading = true;

        WWW = new WWW(url);
        yield return WWW;

        isDownloading = false;

        if (WWW == null)
        {
            Debug.Log("WWW is null!");
            resource.ioErrorHandler();
        }

        if (WWW.error == null)
        {
            Debug.Log("Download is finished!" + WWW.text);
            resource.data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(WWW.text);
           
            appDataLoadSuccessHandler(resource);
        }

        else
        {
            Debug.Log("Download error : " + WWW.error);
            appDataLoadFailHandler(resource);
            resource.ioErrorHandler();
        }
       // WWW.Dispose();
    }
}
