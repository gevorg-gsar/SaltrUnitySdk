using UnityEngine;
using System.Collections;
using saltr_unity_sdk;
using NUnit.Framework;


public class SLTCellTests
{
    [Test]
    public void test_SLTCellInitialization()
    {
        int col = 10;
        int row = 15;
        SLTCell cell = new SLTCell(col, row);

        Assert.AreEqual(col, cell.col);
        Assert.AreEqual(row, cell.row);
    }
}
