using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using saltr_unity_sdk;
using Assets;
using NUnit.Framework;

public class SLTExperimentTests
{
    [Test]
    public void test_SLTExperiments()
    {
        string tokken = "new Asset";
        string type = "type";
        string partition = "A";
        List<object> customEvents = new List<object>() { "1", "2", "3" };

        SLTExperiment experiment = new SLTExperiment(tokken, partition, type, customEvents);

        Assert.AreEqual(type, experiment.type);
        Assert.AreEqual(tokken, experiment.token);
        Assert.AreEqual(partition, experiment.partition);

    }
}
