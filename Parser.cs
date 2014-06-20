using UnityEngine;
using System.Collections;
using saltr_unity_sdk;
using System.Collections.Generic;
using LitJson;
using saltr_unity_sdk;
using Assets.Boomlagoon.JSON;
using MiniJSON;
using Assets;


public class Parser : MonoBehaviour
{
    void Start()
    {
        SLTUnity saltar = new SLTUnity("a78daba7-89fe-eb1a-7153-3700f699ee4e", "ITA447442", false);
        saltar.start();
        saltar.connect(Success, Fail);
    }

    public void Success(SLTResource res)
    {
        Debug.Log("Success!");

    }

    public void Fail(SLTResource res)
    {
        Debug.Log("Fail");
    }
}
