using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using saltr_unity_sdk;
using Assets;
using NUnit.Framework;


public class SLTLevelSettingsTests
{
    [Test]
    public void test_SLTLevelSettingsInitialization()
    {

        Dictionary<string, object> assetMap = new Dictionary<string, object>();
        assetMap["key1"] = "Asset1";
        assetMap["key2"] = "Asset2";
        assetMap["key3"] = "Asset3";

        Dictionary<string, object> stateMap = new Dictionary<string, object>();
        stateMap["key1"] = "State1";
        stateMap["key2"] = "State2";
        stateMap["key3"] = "State3";

        SLTLevelSettings levelSettings = new SLTLevelSettings(assetMap, stateMap);

        Assert.AreEqual(assetMap, levelSettings.assetMap);
        Assert.AreEqual(stateMap, levelSettings.stateMap);

    }
}
