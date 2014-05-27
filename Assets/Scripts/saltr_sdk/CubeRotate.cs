using UnityEngine;
using System.Collections;

public class CubeRotate : MonoBehaviour
{
    // Use this for initialization

    Texture2D texture = null;
    IEnumerator Start()
    {
        string url = "http://fc03.deviantart.net/fs30/i/2008/122/4/5/Crack_Texture_by_struckdumb.jpg";
        // Start a download of the given URL
        WWW www = new WWW(url);
        // Wait for download to complete
        yield return www;
        // assign texture
        this.texture = www.texture;
    }

    void Update()
    {
       // transform.Rotate(new Vector3(20, 0));
        if (texture != null)
        {
            renderer.material.mainTexture = texture;
            texture = null;
        }
    }
}
