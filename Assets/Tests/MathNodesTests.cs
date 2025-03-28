using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class MathNodesTests : InteractivityTestsHelpers
{
    [Test]
    public void TestE()
    {
        TestOperationResult("math/e", math.E);
    }

    [Test]
    public void TestPI()
    {
        TestOperationResult("math/pi", math.PI);
    }

    [Test]
    public void TestInf()
    {
        TestOperationResult("math/inf", math.INFINITY);
    }

    [Test]
    public void TestNAN()
    {
        TestOperationResult("math/nan", math.NAN);
    }

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
        TestOperationResult("math/eq", tv1.x, tv2.x, false);
        TestOperationResult("math/eq", tv1.xy, tv2.xy, false);
        TestOperationResult("math/eq", tv1.xyz, tv2.xyz, false);
        TestOperationResult("math/eq", tv1, tv2, false);

        TestOperationResult("math/eq", tv1.x, tv1.x, true);
        TestOperationResult("math/eq", tv1.xy, tv1.xy, true);
        TestOperationResult("math/eq", tv1.xyz, tv1.xyz, true);
        TestOperationResult("math/eq", tv1, tv1, true);

        TestOperationResult("math/eq", tv1i, tv2i, false);
        TestOperationResult("math/eq", tv1i, tv1i, true);

        TestOperationResult("math/eq", tv1b, tv2b, false);
        TestOperationResult("math/eq", tv1b, tv1b, true);
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
    public void TestLength()
    {
        TestOperationResult("math/length", tv1.xy, math.length(tv1.xy));
        TestOperationResult("math/length", tv1.xyz, math.length(tv1.xyz));
        TestOperationResult("math/length", tv1, math.length(tv1));
    }

    [Test]
    public void TestNormalize()
    {
        TestOperationResult("math/normalize", tv1.xy, math.normalize(tv1.xy));
        TestOperationResult("math/normalize", tv1.xyz, math.normalize(tv1.xyz));
        TestOperationResult("math/normalize", tv1, math.normalize(tv1));
    }

    [Test]
    public void TestDot()
    {
        TestOperationResult("math/dot", tv1.xy, tv2.xy, math.dot(tv1.xy, tv2.xy));
        TestOperationResult("math/dot", tv1.xyz, tv2.xyz, math.dot(tv1.xyz, tv2.xyz));
        TestOperationResult("math/dot", tv1, tv2, math.dot(tv1, tv2));
    }

    [Test]
    public void TestCross()
    {
        TestOperationResult("math/cross", tv1.xyz, tv2.xyz, math.cross(tv1.xyz, tv2.xyz));
        TestOperationResult("math/cross", tv1.xyz, tv1.xyz, new float3(0.0f, 0.0f, 0.0f));
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
