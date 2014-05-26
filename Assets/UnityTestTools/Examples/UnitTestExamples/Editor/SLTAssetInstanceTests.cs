using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using saltr_unity_sdk;

public class SLTAssetInstanceTests
{
    [Test]
    public void test_SLTAssetInstanceInitialization()
    {
        string tokken = "tokken";
        string state = "state";
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties["key1"] = "argument1";
        properties["key2"] = "argument2";
        SLTAssetInstance assetInstance = new SLTAssetInstance(tokken, state, properties);

        Assert.AreEqual(tokken, assetInstance.token);
        Assert.AreEqual(state, assetInstance.state);
        Assert.AreEqual(properties, assetInstance.properties);


    }
}
