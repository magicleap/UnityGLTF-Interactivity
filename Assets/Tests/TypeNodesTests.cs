using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class TypeNodesTests : InteractivityTestsHelpers
{
    [Test]
    public void TestIntToFloat()
    {
        TestOperationResult("type/intToFloat", tv1i, (float)tv1i);
    }

    [Test]
    public void TestFloatToInt()
    {
        TestOperationResult("type/floatToInt", tv1.x, (int)tv1.x);
    }

    [Test]
    public void TestBoolToInt()
    {
        TestOperationResult("type/boolToInt", tv1b, tv1b ? 1 : 0);
    }

    [Test]
    public void TestBoolToFloat()
    {
        TestOperationResult("type/boolToFloat", tv1b, tv1b ? 1.0f : 0.0f);
    }

    [Test]
    public void TestIntToBool()
    {
        TestOperationResult("type/intToBool", tv1i, tv1i != 0);
    }

    [Test]
    public void TestFloatToBool()
    {
        TestOperationResult("type/floatToBool", tv1.x, tv1.x != 0.0f);
    }
}
