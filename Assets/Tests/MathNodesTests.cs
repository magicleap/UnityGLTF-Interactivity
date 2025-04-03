using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityGLTF.Interactivity;

public class MathNodesTests : InteractivityTestsHelpers
{
    [Test]
    public void TestAsr()
    {
        TestOperationResult("math/asr", 17, 1, 8);
        TestOperationResult("math/asr", -19, 2, -5);
    }

    [Test]
    public void TestLsl()
    {
        TestOperationResult("math/lsl", 25, 2, 100);
        TestOperationResult("math/lsl", -23, 2, -92);
    }

    [Test]
    public void TestClz()
    {
        TestOperationResult("math/clz", 32, 26);
        TestOperationResult("math/clz", 1, 31);
        TestOperationResult("math/clz", 0x0fff0000, 4);
    }

    [Test]
    public void TestCtz()
    {
        TestOperationResult("math/ctz", 1, 0);
        TestOperationResult("math/ctz", 16, 4);
        TestOperationResult("math/ctz", 0x0f000000, 24);
    }

    [Test]
    public void TestPopcnt()
    {
        TestOperationResult("math/popcnt", 0b0000001000100000, 2);
    }

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
    public void TestSign()
    {
        TestOperationResult("math/sign", 32.0f, 1.0f);
        TestOperationResult("math/sign", -12.0f, -1.0f);
        TestOperationResult("math/sign", 0.0f, 0.0f);

        TestOperationResult("math/sign", 32, 1);
        TestOperationResult("math/sign", -12, -1);
        TestOperationResult("math/sign", 0, 0);

        TestOperationResultAllFloats("math/sign", new float4(32.0f, -12.0f, 0.0f, 5.0f), new float4(1.0f, -1.0f, 0.0f, 1.0f));
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
    public void TestDiv()
    {
        TestOperationResult("math/div", 5.0f, 0.0f, math.INFINITY);
        TestOperationResult("math/div", 5.0f, math.INFINITY, 0.0f);
        TestOperationResult("math/div", 12.4f, 4.0f, 3.1f);

        TestOperationResultAllFloats("math/div", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(4.0f, math.INFINITY, -2.28571429f, 0.5f));
    }

    [Test]
    public void TestRem()
    {
        TestOperationResult("math/rem", 5.0f, 0.0f, math.NAN);
        TestOperationResult("math/rem", 5.0f, math.INFINITY, 5.0f);

        using(new StdThresholdCompare(this))
        {
            TestOperationResult("math/rem", 12.4f, 4.0f, 0.4f);
        }

        TestOperationResultAllFloats("math/rem", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(0.0f, math.NAN, -4.0f, 7.0f));
    }

    [Test]
    public void TestMin()
    {
        TestOperationResultAllFloats("math/min", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(3.0f, 0.0f, -32.0f, 7.0f));

        TestOperationResult("math/min", 100, 10, 10);
    }

    [Test]
    public void TestMax()
    {
        TestOperationResultAllFloats("math/max", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(12.0f, 16.0f, 14.0f, 14.0f));

        TestOperationResult("math/max", 100, 10, 100);
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
    public void TestMix()
    {
        TestOperationResultAllFloats("math/mix", new float4(1.0f, 2.0f, 3.0f, 4.0f), new float4(9.0f, 10.0f, 11.0f, 12.0f), new float4(1.0f, 0.25f, 0.5f, 0.0f), new float4(9.0f, 4.0f, 7.0f, 4.0f));
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
    public void TestLT()
    {
        TestOperationResult("math/lt", 10.0f, 20.0f, true);
        TestOperationResult("math/lt", 40.0f, 20.0f, false);

        TestOperationResult("math/lt", 10, 20, true);
        TestOperationResult("math/lt", 40, 20, false);
    }

    [Test]
    public void TestLE()
    {
        TestOperationResult("math/le", 10.0f, 20.0f, true);
        TestOperationResult("math/le", 10.0f, 10.0f, true);
        TestOperationResult("math/le", 40.0f, 20.0f, false);

        TestOperationResult("math/le", 10, 20, true);
        TestOperationResult("math/le", 10, 10, true);
        TestOperationResult("math/le", 40, 20, false);
    }

    [Test]
    public void TestGT()
    {
        TestOperationResult("math/gt", 10.0f, 20.0f, false);
        TestOperationResult("math/gt", 40.0f, 20.0f, true);

        TestOperationResult("math/gt", 10, 20, false);
        TestOperationResult("math/gt", 40, 20, true);
    }

    [Test]
     public void TestGE()
    {
        TestOperationResult("math/ge", 10.0f, 20.0f, false);
        TestOperationResult("math/ge", 10.0f, 10.0f, true);
        TestOperationResult("math/ge", 40.0f, 20.0f, true);

        TestOperationResult("math/ge", 10, 20, false);
        TestOperationResult("math/ge", 10, 10, true);
        TestOperationResult("math/ge", 40, 20, true);
    }

    [Test]
    public void TestIsNan()
    {
        TestOperationResult("math/isnan", (float)math.acos(-2.0), true);
        TestOperationResult("math/isnan", 10.0f, false);
    }

    [Test]
    public void TestIsInf()
    {
        TestOperationResult("math/isinf", 10.0f / 0.0f, true);
        TestOperationResult("math/isinf", 10.0f, false);
    }
    
    [Test]
    public void TestSelect()
    {
        TestOperationResultWithCondition("math/select", 10.0f, 20.0f, true, 10.0f);
        TestOperationResultWithCondition("math/select", 10.0f, 20.0f, false, 20.0f);

        TestOperationResultWithCondition("math/select", "A",  "B", true, "A");
        TestOperationResultWithCondition("math/select", "A",  "B", false, "B");
    }

    [Test]
    public void TestSin()
    {
        TestMathOpAllFloats1op("math/sin", math.sin(tv1));
        TestOperationResult("math/sin", 0.0f, 0.0f);
        TestOperationResult("math/sin", (float)(math.PI / 2.0), 1.0f);
    }

    [Test]
    public void TestCos()
    {
        TestMathOpAllFloats1op("math/cos", math.cos(tv1));
        TestOperationResult("math/cos", 0.0f, 1.0f);

        using(new StdThresholdCompare(this))
        {
            TestOperationResult("math/cos", (float)(math.PI / 2.0), 0.0f);
        }
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
    public void TestSinH()
    {
        TestMathOpAllFloats1op("math/sinh", math.sinh(tv1));
        TestOperationResult("math/sinh", 0.0f, 0.0f);
    }

    [Test]
    public void TestCosH()
    {
        TestMathOpAllFloats1op("math/cosh", math.cosh(tv1));
        TestOperationResult("math/cosh", 0.0f, 1.0f);
    }

    [Test]
    public void TestTanH()
    {
        TestMathOpAllFloats1op("math/tanh", math.tanh(tv1));
        TestOperationResult("math/tanh", math.INFINITY, 1.0f);
        TestOperationResult("math/tanh", -math.INFINITY, -1.0f);
    }

    private float4 asinh(float4 x)
    {
        return math.log(x + math.sqrt(x * x + 1));
    }

    private float4 acosh(float4 x)
    {
        return math.log(x + math.sqrt(x * x - 1));
    }

    private float4 atanh(float4 x)
    {
        return 0.5f * math.log((1 + x) / (1 - x));
    }

    [Test]
    public void TestASinH()
    {
        TestMathOpAllFloats1op("math/asinh", asinh(tv1));
    }

    [Test]
    public void TestACosH()
    {
        float4 val = new float4(30.0f, 10.0f, 0.0f, 15.0f);
        TestOperationResultAllFloats("math/acosh", val, acosh(val));
        TestOperationResult("math/acosh", 1.0f, 0.0f);
    }

    [Test]
    public void TestATanH()
    {
        float4 val = new float4(30.0f, 0.3f, -0.2f, 15.0f);
        TestOperationResultAllFloats("math/atanh", val, atanh(val));
        TestOperationResult("math/atanh", 1.0f, math.INFINITY);
        TestOperationResult("math/atanh", -1.0f, -math.INFINITY);
    }

    [Test]
    public void TestExp()
    {
        TestMathOpAllFloats1op("math/exp", math.exp(tv1));
    }

    [Test]
    public void TestLog()
    {
        float4 val = new float4(30.0f, 0.3f, -0.2f, 15.0f);
        TestOperationResult("math/log", val, math.log(val));
        TestOperationResult("math/log", -1.0f, math.NAN);
        TestOperationResult("math/log", 0.0f, -math.INFINITY);
        TestOperationResult("math/log", 1.0f, 0.0f);
    }

    [Test]
    public void TestLog2()
    {
        float4 val = new float4(30.0f, 0.3f, -0.2f, 15.0f);
        TestOperationResultAllFloats("math/log2", val, math.log2(val));
        TestOperationResult("math/log2", -1.0f, math.NAN);
        TestOperationResult("math/log2", 0.0f, -math.INFINITY);
        TestOperationResult("math/log2", 1.0f, 0.0f);
    }

    [Test]
    public void TestLog10()
    {
        float4 val = new float4(30.0f, 0.3f, -0.2f, 15.0f);
        TestOperationResultAllFloats("math/log10", val, math.log10(val));
        TestOperationResult("math/log10", -1.0f, math.NAN);
        TestOperationResult("math/log10", 0.0f, -math.INFINITY);
        TestOperationResult("math/log10", 1.0f, 0.0f);
    }

    [Test]
    public void TestSqrt()
    {
        float4 val = new float4(30.0f, 0.3f, -0.2f, 15.0f);
        TestOperationResultAllFloats("math/sqrt", val, math.sqrt(val));
        TestOperationResult("math/sqrt", -1.0f, math.NAN);
        TestOperationResult("math/sqrt", 0.0f, 0.0f);
    }

    [Test]
    public void TestPow()
    {
        float4 val = new float4(30.0f, 0.3f, -0.2f, 15.0f);
        float4 e = new float4(10.0f, 1.3f, -1.2f, 5.0f);

        TestOperationResultAllFloats("math/pow", val, e, math.pow(val, e));
        TestOperationResult("math/pow", -1.0f, 2.0f, 1.0f);
        TestOperationResult("math/pow", 0.0f, 1000.0f, 0.0f);
        TestOperationResult("math/pow", 1000.0f, 0.0f, 1.0f);
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
    public void TestCombine2()
    {
        TestOperationResult("math/combine2", 1.0f, 2.0f, new float2(1.0f, 2.0f));
    }

    [Test]
    public void TestCombine3()
    {
        TestOperationResult("math/combine3", 1.0f, 2.0f, 3.0f, new float3(1.0f, 2.0f, 3.0f));
    }

    [Test]
    public void TestCombine4()
    {
        TestOperationResult("math/combine4", 1.0f, 2.0f, 3.0f, 4.0f, new float4(1.0f, 2.0f, 3.0f, 4.0f));
    }

    [Test]
    public void TestCombine2x2()
    {
        TestOperationResult("math/combine2x2", new float[] {1.0f, 2.0f, 3.0f, 4.0f}, new float2x2(1.0f, 2.0f, 3.0f, 4.0f));
    }

    [Test]
    public void TestCombine3x3()
    {
        TestOperationResult("math/combine3x3", new float[] {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f}, new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f));
    }

    [Test]
    public void TestCombine4x4()
    {
        TestOperationResult("math/combine4x4", new float[] {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f}, new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f));
    }

    [Test]
    public void TestAnd()
    {
        TestOperationResult("math/and", true, false, false);
        TestOperationResult("math/and", false, false, false);
        TestOperationResult("math/and", true, true, true);

        TestOperationResult("math/and", 3, 8, 3 & 8);
    }

    [Test]
    public void TestOr()
    {
        TestOperationResult("math/or", true, false, true);
        TestOperationResult("math/or", false, false, false);
        TestOperationResult("math/or", true, true, true);

        TestOperationResult("math/or", 3, 8, 3 | 8);
    }

    [Test]
    public void TestXor()
    {
        TestOperationResult("math/xor", true, false, true);
        TestOperationResult("math/xor", false, false, false);
        TestOperationResult("math/xor", true, true, false);

        TestOperationResult("math/xor", 3, 8, 3 ^ 8);
    }

    [Test]
    public void TestRotate3d()
    {
        using(new StdThresholdCompare(this))
        {
            TestOperationResult("math/rotate3d", (n)=>
            {
                n.AddValue(ConstStrings.A, new float3(0.0f, 0.0f, -1.0f));
                n.AddValue(ConstStrings.B, new float3(0.0f, 1.0f, 0.0f));
                n.AddValue(ConstStrings.C, math.PI * 0.5f);
            }, new float3(-1.0f, 0.0f, 0.0f));

            TestOperationResult("math/rotate3d", (n)=>
            {
                n.AddValue(ConstStrings.A, new float3(1.0f, 0.0f, 0.0f));
                n.AddValue(ConstStrings.B, new float3(0.0f, 1.0f, 0.0f));
                n.AddValue(ConstStrings.C, math.PI);
            }, new float3(-1.0f, 0.0f, 0.0f));
        }
    }

    [Test]
    public void TestRotate2d()
    {
        using(new StdThresholdCompare(this))
        {
            TestOperationResult("math/rotate2d", (n)=>
            {
                n.AddValue(ConstStrings.A, new float2(0.0f, 1.0f));
                n.AddValue(ConstStrings.B, math.PI * 0.5f);
            }, new float2(-1.0f, 0.0f));

            TestOperationResult("math/rotate2d", (n)=>
            {
                n.AddValue(ConstStrings.A, new float2(-1.0f, 0.0f));
                n.AddValue(ConstStrings.B, math.PI * 0.5f);
            }, new float2(0.0f, -1.0f));
        }
    }

    [Test]
    public void TestCbrt()
    {
        using(new StdThresholdCompare(this))
        {
            TestOperationResultAllFloats("math/cbrt", new float4(11.3f, -50.3f, 33.3f, 100.1f), new float4(2.24401703f, -3.69138487f, 3.21722482f, 4.64313551f)); 
        }
    }

    [Test]
    public void TestTrunc()
    {
        TestOperationResultAllFloats("math/trunc", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(15.0f, -10.0f, 12.0f, -32.0f));
    }

    [Test]
    public void TestFract()
    {
        using(new StdThresholdCompare(this))
        {
            TestOperationResultAllFloats("math/fract", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(0.4f, 0.9f, 0.39f, 0.67f));
        }
    }

    [Test]
    public void TestNeg()
    {
        TestOperationResultAllFloats("math/neg", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(-15.4f, 10.1f, -12.39f, 32.33f));
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
