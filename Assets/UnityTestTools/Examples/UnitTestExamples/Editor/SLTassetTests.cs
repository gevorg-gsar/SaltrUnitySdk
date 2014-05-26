using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using saltr_unity_sdk;
using Assets;
using NUnit.Framework;

public class SLTAssetTests
{
    [Test]
    public void test_SLTAssetInitialization()
    {
        string tokken = "new Asset";
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties["key1"] = "argument1";
        properties["key2"] = "argument2";

        SLTAsset asset = new SLTAsset(tokken, properties);

        Assert.AreEqual(tokken, asset.token);
        Assert.AreEqual(properties, asset.properties);

    }
}
