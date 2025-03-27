using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class MathNodesTests : InteractivityTestsHelpers
{
    [Test]
    public void TestAbs()
    {
        TestMathOpAllFloats1op("math/abs", math.abs(tv1));
        TestMathOpInt1op("math/abs", (int)math.abs(tv1i));
    }

    [Test]
    public void TestFloor()
    {
        TestMathOpAllFloats1op("math/floor", math.floor(tv1));
    }

    [Test]
    public void TestCeil()
    {
        TestMathOpAllFloats1op("math/ceil", math.ceil(tv1));
    }

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
    public void TestTan()
    {
        TestMathOpAllFloats1op("math/tan", math.tan(tv1));
        // TestOperationResult("math/tan", math.PI * 0.5f, math.INFINITY); // it fails because of PI not precise
    }

    [Test]
    public void TestAsin()
    {
        TestMathOpAllFloats1op("math/asin", math.asin(tv1));

        TestOperationResult("math/asin", 1000.0f, math.NAN);
        TestOperationResult("math/asin", 1.0f, math.PI * 0.5f);
    }

    [Test]
    public void TestAcos()
    {
        TestMathOpAllFloats1op("math/acos", math.acos(tv1));

        TestOperationResult("math/acos", 1000.0f, math.NAN);
        TestOperationResult("math/acos", 0.0f, math.PI * 0.5f);
    }

    [Test]
    public void TestAtan()
    {
        TestMathOpAllFloats1op("math/atan", math.atan(tv1));

        TestOperationResult("math/atan", 1.0f, math.PI / 4.0f);
        TestOperationResult("math/atan", 0.0f, 0.0f);
    }

    [Test]
    public void TestAtan2()
    {
        TestMathOpAllFloats2op("math/atan2", math.atan2(tv1, tv2));

        TestOperationResult("math/atan2", 1.0f, 1.0f, math.PI / 4.0f);
        TestOperationResult("math/atan2", 1.0f, 0.0f, math.PI / 2.0f);
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
}
