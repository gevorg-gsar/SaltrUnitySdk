using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using saltr_unity_sdk;
using Assets;
using NUnit.Framework;


public class SLTLevelPackTests
{
    [Test]
    public void tes_SLTLevelPackInitialization()
    {
        string token = "token";
        int index = 78;
        List<SLTLevel> levels = new List<SLTLevel>() { new SLTLevel("7", 4, "kkj", "ss", "007") };
        SLTLevelPack levelPack = new SLTLevelPack(token, index, levels);

        Assert.AreEqual(token, levelPack.token);
        Assert.AreEqual(index, levelPack.index);
        Assert.AreEqual(levels, levelPack.levels);
    }
}
