using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using saltr_unity_sdk;
using NUnit.Framework;

public class SLTFeatureTests
{
    [Test]
    public void test_SLTFeaturesInitialization()
    {
        string tokken = "new Asset";
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties["key1"] = "argument1";
        properties["key2"] = "argument2";

        SLTFeature feature = new SLTFeature(tokken, properties, true);
        Assert.AreEqual(tokken, feature.token);
        Assert.AreEqual(properties, feature.properties);
        Assert.AreEqual(true, feature.required);

    }
}
