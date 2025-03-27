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
}
