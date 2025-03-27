using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class TypeNodesTests : InteractivityTestsHelpers
{
    [Test]
    public void TestFloatToInt()
    {
        TestOperationResultT("type/floatToInt", tv1.x, (int)tv1.x);
    }

    [Test]
    public void TestBoolToInt()
    {
        TestOperationResultT("type/boolToInt", tv1b, tv1b ? 1 : 0);
    }

    [Test]
    public void TestBoolToFloat()
    {
        TestOperationResultT("type/boolToFloat", tv1b, tv1b ? 1.0f : 0.0f);
    }

    [Test]
    public void TestIntToBool()
    {
        TestOperationResultT("type/intToBool", tv1i, tv1i != 0);
    }

    [Test]
    public void TestFloatToBool()
    {
        TestOperationResultT("type/floatToBool", tv1.x, tv1.x != 0.0f);
    }
}
