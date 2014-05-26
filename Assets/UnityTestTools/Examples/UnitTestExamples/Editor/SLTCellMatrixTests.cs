using UnityEngine;
using System.Collections;
using saltr_unity_sdk;
using System.Collections.Generic;
using NUnit.Framework;


public class SLTCellMatrixTests
{
    [Test]
    public void SLTCellMatrixInitialization()
    {
        int width = 50;
        int height = 11;

        SLTCellMatrix matrix = new SLTCellMatrix(width, height);

        Assert.AreEqual(width, matrix.width);
        Assert.AreEqual(height, matrix.height);

    }
}
