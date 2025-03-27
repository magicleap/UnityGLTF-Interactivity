using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class MathNodesTests : InteractivityTestsHelpers
{
    [Test]
    public void TestAdd()
    {
        TestMathOpAllFloats2op("math/add", tv1 + tv2);
        TestMathOpInt2op("math/add", tv1i + tv2i);
    }

    [Test]
    public void TestSub()
    {
        TestMathOpAllFloats2op("math/sub", tv1 - tv2);
        TestMathOpInt2op("math/sub", tv1i - tv2i);
    }

    [Test]
    public void TestMul()
    {
        TestMathOpAllFloats2op("math/mul", tv1 * tv2);
        TestMathOpInt2op("math/mul", tv1i * tv2i);
    }

    [Test]
    public void TestClamp()
    {
        TestMathOpAllFloats3op("math/clamp", math.clamp(tv1, tv2, tv3));
    }

    [Test]
    public void TestSaturate()
    {
        TestMathOpAllFloats1op("math/saturate", math.saturate(tv1));
    }

    [Test]
    public void TestCeil()
    {
        TestMathOpAllFloats1op("math/ceil", math.ceil(tv1));
    }

    [Test]
    public void TestFloor()
    {
        TestMathOpAllFloats1op("math/floor", math.floor(tv1));
    }

    [Test]
    public void TestRad()
    {
        TestMathOpAllFloats1op("math/rad", math.radians(tv1));
    }

    [Test]
    public void TestDeg()
    {
        TestMathOpAllFloats1op("math/deg", math.degrees(tv1));
    }

    [Test]
    public void TestEq()
    {
        TestOperationResultT("math/eq", tv1.x, tv2.x, false);
        TestOperationResultT("math/eq", tv1.xy, tv2.xy, false);
        TestOperationResultT("math/eq", tv1.xyz, tv2.xyz, false);
        TestOperationResultT("math/eq", tv1, tv2, false);

        TestOperationResultT("math/eq", tv1.x, tv1.x, true);
        TestOperationResultT("math/eq", tv1.xy, tv1.xy, true);
        TestOperationResultT("math/eq", tv1.xyz, tv1.xyz, true);
        TestOperationResultT("math/eq", tv1, tv1, true);

        TestOperationResultT("math/eq", tv1i, tv2i, false);
        TestOperationResultT("math/eq", tv1i, tv1i, true);

        TestOperationResultT("math/eq", tv1b, tv2b, false);
        TestOperationResultT("math/eq", tv1b, tv1b, true);
    }

    [Test]
    public void TestAbs()
    {
        TestMathOpAllFloats1op("math/abs", math.abs(tv1));
        TestMathOpInt1op("math/abs", (int)math.abs(tv1i));
    }
}
