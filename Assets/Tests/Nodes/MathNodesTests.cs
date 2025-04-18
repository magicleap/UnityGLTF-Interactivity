using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityGLTF.Interactivity.Tests
{
    public class MathNodesTests : NodeTestHelpers
    {
        [Test]
        public void TestAsr()
        {
            TestNode("math/asr", 17, 1, 8);
            TestNode("math/asr", -19, 2, -5);
        }

        [Test]
        public void TestLsl()
        {
            TestNode("math/lsl", 25, 2, 100);
            TestNode("math/lsl", -23, 2, -92);
        }

        [Test]
        public void TestClz()
        {
            TestNode("math/clz", 32, 26);
            TestNode("math/clz", 1, 31);
            TestNode("math/clz", 0x0fff0000, 4);
        }

        [Test]
        public void TestCtz()
        {
            TestNode("math/ctz", 1, 0);
            TestNode("math/ctz", 16, 4);
            TestNode("math/ctz", 0x0f000000, 24);
        }

        [Test]
        public void TestPopcnt()
        {
            TestNode("math/popcnt", 0b0000001000100000, 2);
        }

        [Test]
        public void TestE()
        {
            TestNode("math/e", math.E);
        }

        [Test]
        public void TestPI()
        {
            TestNode("math/pi", math.PI);
        }

        [Test]
        public void TestInf()
        {
            TestNode("math/inf", math.INFINITY, ComparisonType.IsInfinity);
        }

        [Test]
        public void TestNAN()
        {
            TestNode("math/nan", math.NAN, ComparisonType.IsNaN);
        }

        [Test]
        public void TestAbs()
        {
            TestNode("math/abs", -2, 2);
            TestNode("math/abs", 9, 9);

            TestNodeWithAllFloatNInputVariants("math/abs", new float4(-2f, 2f, -9.15f, 0f), new float4(2f, 2f, 9.15f, 0f));
        }

        [Test]
        public void TestSign()
        {
            TestNode("math/sign", 32.0f, 1.0f);
            TestNode("math/sign", -12.0f, -1.0f);
            TestNode("math/sign", 0.0f, 0.0f);

            TestNode("math/sign", 32, 1);
            TestNode("math/sign", -12, -1);
            TestNode("math/sign", 0, 0);

            TestNodeWithAllFloatNInputVariants("math/sign", new float4(32.0f, -12.0f, 0.0f, 5.0f), new float4(1.0f, -1.0f, 0.0f, 1.0f));
        }

        [Test]
        public void TestFloor()
        {
            TestNodeWithAllFloatNInputVariants("math/floor", new float4(3.87f, 3.14f, -3.14f, -3.87f), new float4(3f, 3f, -4f, -4f));
        }

        [Test]
        public void TestTrunc()
        {
            TestNodeWithAllFloatNInputVariants("math/trunc", new float4(3.87f, 3.14f, -3.14f, -3.87f), new float4(3f, 3f, -3f, -3f));
        }

        [Test]
        public void TestCeil()
        {
            TestNodeWithAllFloatNInputVariants("math/ceil", new float4(3.87f, 3.14f, -3.14f, -3.87f), new float4(4f, 4f, -3f, -3f));
        }

        [Test]
        public void TestAdd()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(-34.0f, 22f, -11f, 70.0f);

            var expected = a + b;

            TestNodeWithAllFloatNInputVariants("math/add", a, b, expected);
            TestNode("math/add", 5, 15, 20);
            TestNode("math/add", 5, -15, -10);
            TestNode("math/add", 5, 0, 5);
            TestNode("math/add", 0, 0, 0);
        }

        [Test]
        public void TestSub()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(-34.0f, 22f, -11f, 70.0f);

            var expected = a - b;

            TestNodeWithAllFloatNInputVariants("math/sub", a, b, expected);
            TestNode("math/sub", 5, 15, -10);
            TestNode("math/sub", 5, -15, 20);
            TestNode("math/sub", 5, 0, 5);
            TestNode("math/sub", 0, 0, 0);
        }

        [Test]
        public void TestMul()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(-34.0f, 22f, -11f, 70.0f);

            var expected = a * b;

            TestNodeWithAllFloatNInputVariants("math/mul", a, b, expected);
            TestNode("math/mul", 1, 15, 15);
            TestNode("math/mul", 0, -15, 0);
            TestNode("math/mul", 5, 0, 0);
            TestNode("math/mul", 5, 5, 25);
            TestNode("math/mul", 5, -5, -25);
        }

        [Test]
        public void TestDiv()
        {
            var a = new float4(5f, 5f, 12.4f, -12.4f);
            var b = new float4(1f, 12f, -55f, -12.4f);
            var expected = a / b;
            TestNodeWithAllFloatNInputVariants("math/div", a, b, expected);
            TestNode("math/div", 5f, 0f, float.PositiveInfinity, ComparisonType.IsInfinity);
            TestNode("math/div", 5f, float.PositiveInfinity, 0f);
        }

        [Test]
        public void TestRem()
        {
            var a = new float4(5f, 5f, 12.4f, -12.4f);
            var b = new float4(1f, 12f, -55f, -12.4f);
            var expected = a % b;

            TestNodeWithAllFloatNInputVariants("math/rem", 5.0f, 0.0f, math.NAN, ComparisonType.IsNaN);
            TestNodeWithAllFloatNInputVariants("math/rem", 5.0f, math.INFINITY, 5.0f, ComparisonType.Equals);
            TestNodeWithAllFloatNInputVariants("math/rem", 12.4f, 4.0f, 0.4f);
            TestNodeWithAllFloatNInputVariants("math/rem", a, b, expected);
            TestNode("math/rem", 5, 4, 1);
            TestNode("math/rem", 5, 5, 0);
        }

        [Test]
        public void TestMin()
        {
            TestNodeWithAllFloatNInputVariants("math/min", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(3.0f, 0.0f, -32.0f, 7.0f));

            TestNode("math/min", 100, 10, 10);
        }

        [Test]
        public void TestMax()
        {
            TestNodeWithAllFloatNInputVariants("math/max", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(12.0f, 16.0f, 14.0f, 14.0f));

            TestNode("math/max", 100, 10, 100);
        }

        [Test]
        public void TestClamp()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);
            var c = new float4(92.0f, 43.0f, 59.0f, 90.0f);

            var expected = math.clamp(a, b, c);

            TestNodeWithAllFloatNInputVariants("math/clamp", a, b, c, expected);
        }

        [Test]
        public void TestSaturate()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            TestNodeWithAllFloatNInputVariants("math/saturate", a, math.saturate(a));
        }

        [Test]
        public void TestMix()
        {
            TestNodeWithAllFloatNInputVariants("math/mix", new float4(1.0f, 2.0f, 3.0f, 4.0f), new float4(9.0f, 10.0f, 11.0f, 12.0f), new float4(1.0f, 0.25f, 0.5f, 0.0f), new float4(9.0f, 4.0f, 7.0f, 4.0f));
        }

        [Test]
        public void TestEq()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            TestNode("math/eq", a.x, b.x, false);
            TestNode("math/eq", a.xy, b.xy, false);
            TestNode("math/eq", a.xyz, b.xyz, false);
            TestNode("math/eq", a, b, false);

            TestNode("math/eq", a.x, a.x, true);
            TestNode("math/eq", a.xy, a.xy, true);
            TestNode("math/eq", a.xyz, a.xyz, true);
            TestNode("math/eq", a, a, true);

            TestNode("math/eq", 1, 2, false);
            TestNode("math/eq", 1, 1, true);

            TestNode("math/eq", -2, 2, false);
            TestNode("math/eq", -2, -2, true);
        }

        [Test]
        public void TestLT()
        {
            TestNode("math/lt", 10.0f, 20.0f, true);
            TestNode("math/lt", 40.0f, 20.0f, false);

            TestNode("math/lt", 10, 20, true);
            TestNode("math/lt", 40, 20, false);
        }

        [Test]
        public void TestLE()
        {
            TestNode("math/le", 10.0f, 20.0f, true);
            TestNode("math/le", 10.0f, 10.0f, true);
            TestNode("math/le", 40.0f, 20.0f, false);

            TestNode("math/le", 10, 20, true);
            TestNode("math/le", 10, 10, true);
            TestNode("math/le", 40, 20, false);
        }

        [Test]
        public void TestGT()
        {
            TestNode("math/gt", 10.0f, 20.0f, false);
            TestNode("math/gt", 40.0f, 20.0f, true);

            TestNode("math/gt", 10, 20, false);
            TestNode("math/gt", 40, 20, true);
        }

        [Test]
        public void TestGE()
        {
            TestNode("math/ge", 10.0f, 20.0f, false);
            TestNode("math/ge", 10.0f, 10.0f, true);
            TestNode("math/ge", 40.0f, 20.0f, true);

            TestNode("math/ge", 10, 20, false);
            TestNode("math/ge", 10, 10, true);
            TestNode("math/ge", 40, 20, true);
        }

        [Test]
        public void TestIsNan()
        {
            TestNode("math/isnan", (float)math.acos(-2.0), true);
            TestNode("math/isnan", 10.0f, false);
        }

        [Test]
        public void TestIsInf()
        {
            TestNode("math/isinf", 10.0f / 0.0f, true);
            TestNode("math/isinf", 10.0f, false);
        }

        [Test]
        public void TestSelect()
        {
            MathSelectTest(10.0f, 20.0f, true, 10.0f);
            MathSelectTest(10.0f, 20.0f, false, 20.0f);

            var a = new float4(1f, 2f, 3f, 4f);
            var b = new float4(4f, 3f, 2f, 1f);
            MathSelectTest(a, b, true, a);
            MathSelectTest(a, b, false, b);
        }

        private static void MathSelectTest<T>(T a, T b, bool condition, T expected)
        {
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.CONDITION, new Value() { id = ConstStrings.CONDITION, property = new Property<bool>(condition) });
            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<T>(a) });
            inputs.Add(ConstStrings.B, new Value() { id = ConstStrings.B, property = new Property<T>(b) });
            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            TestNode("math/select", inputs, outputs, ComparisonType.Approximately);
        }

        [Test]
        public void TestSin()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.sin(a);

            TestNodeWithAllFloatNInputVariants("math/sin", a, expected);
            TestNode("math/sin", 0.0f, 0.0f);
            TestNode("math/sin", (float)(math.PI / 2.0), 1.0f);
        }

        [Test]
        public void TestCos()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.cos(a);

            TestNodeWithAllFloatNInputVariants("math/cos", a, expected);
            TestNode("math/cos", 0.0f, 1.0f, ComparisonType.Approximately);
            TestNode("math/cos", (float)(math.PI / 2.0), 0.0f, ComparisonType.Approximately);
        }

        [Test]
        public void TestTan()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.tan(a);

            TestNodeWithAllFloatNInputVariants("math/tan", a, expected);
            // TestNode("math/tan", math.PI * 0.5f, math.INFINITY); // it fails because of PI not precise
        }

        [Test]
        public void TestAsin()
        {
            var a = new float4(-1f, 1f, 0f, 0.4f);
            var expected = new float4(-math.PI / 2f, math.PI / 2f, 0f, 0.411516f);

            TestNodeWithAllFloatNInputVariants("math/asin", a, expected);

            TestNode("math/asin", 1000.0f, math.NAN, ComparisonType.IsNaN);
        }

        [Test]
        public void TestAcos()
        {
            var a = new float4(0f, 0.999f, -0.9999f, 0.1f);
            var expected = math.acos(a);

            TestNodeWithAllFloatNInputVariants("math/acos", a, expected);

            //TestNode("math/acos", 1000.0f, math.NAN, ComparisonType.IsNaN);
            //TestNode("math/acos", 0.0f, math.PI * 0.5f, ComparisonType.Approximately);
        }

        [Test]
        public void TestAtan()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.atan(a);

            TestNodeWithAllFloatNInputVariants("math/atan", a, expected);

            TestNode("math/atan", 1.0f, math.PI / 4.0f);
            TestNode("math/atan", 0.0f, 0.0f);
        }

        [Test]
        public void TestAtan2()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            var expected = math.atan2(a, b);

            TestNodeWithAllFloatNInputVariants("math/atan2", a, b, expected);

            TestNode("math/atan2", 1.0f, 1.0f, math.PI / 4.0f);
            TestNode("math/atan2", 1.0f, 0.0f, math.PI / 2.0f);
        }

        [Test]
        public void TestSinH()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.sinh(a);

            TestNodeWithAllFloatNInputVariants("math/sinh", a, expected);
            TestNode("math/sinh", 0.0f, 0.0f);
        }

        [Test]
        public void TestCosH()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.cosh(a);

            TestNodeWithAllFloatNInputVariants("math/cosh", a, expected);
            TestNode("math/cosh", 0.0f, 1.0f);
        }

        [Test]
        public void TestTanH()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.tanh(a);

            TestNodeWithAllFloatNInputVariants("math/tanh", a, expected);
            TestNode("math/tanh", math.INFINITY, 1.0f);
            TestNode("math/tanh", -math.INFINITY, -1.0f);
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
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = asinh(a);

            TestNodeWithAllFloatNInputVariants("math/asinh", a, expected);
        }

        [Test]
        public void TestACosH()
        {
            float4 val = new float4(1, 10.0f, 100.0f, 1000.0f);
            TestNodeWithAllFloatNInputVariants("math/acosh", val, acosh(val), ComparisonType.Approximately);
            TestNode("math/acosh", 1.0f, 0.0f);
            TestNode("math/acosh", 0.5f, float.NaN, ComparisonType.IsNaN);
        }

        [Test]
        public void TestATanH()
        {
            float4 val = new float4(-0.99f, -0.3f, 0.3f, 0.99f);
            TestNodeWithAllFloatNInputVariants("math/atanh", val, atanh(val));
            TestNode("math/atanh", 1.0f, math.INFINITY, ComparisonType.IsInfinity);
            TestNode("math/atanh", -1.0f, -math.INFINITY, ComparisonType.IsInfinity);
            TestNode("math/atanh", 1.1f, math.NAN, ComparisonType.IsNaN);
            TestNode("math/atanh", -1.1f, -math.NAN, ComparisonType.IsNaN);
        }

        [Test]
        public void TestExp()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.exp(a);

            TestNodeWithAllFloatNInputVariants("math/exp", a, expected);
        }

        [Test]
        public void TestLog()
        {
            float4 val = new float4(30.0f, 0.3f, 1f, 15.0f);
            TestNode("math/log", val, math.log(val), ComparisonType.Approximately);
            TestNode("math/log", -1.0f, math.NAN, ComparisonType.IsNaN);
            TestNode("math/log", 0.0f, -math.INFINITY, ComparisonType.IsInfinity);
        }

        [Test]
        public void TestLog2()
        {
            float4 val = new float4(30.0f, 0.3f, 1f, 15.0f);
            TestNodeWithAllFloatNInputVariants("math/log2", val, math.log2(val));
            TestNode("math/log2", -1.0f, math.NAN, ComparisonType.IsNaN);
            TestNode("math/log2", 0.0f, -math.INFINITY, ComparisonType.IsInfinity);
        }

        [Test]
        public void TestLog10()
        {
            float4 val = new float4(30.0f, 0.3f, 1f, 15.0f);
            TestNodeWithAllFloatNInputVariants("math/log10", val, math.log10(val));
            TestNode("math/log10", -1.0f, math.NAN, ComparisonType.IsNaN);
            TestNode("math/log10", 0.0f, -math.INFINITY, ComparisonType.IsInfinity);
        }

        [Test]
        public void TestSqrt()
        {
            float4 val = new float4(30.0f, 0.3f, 0.2f, 15.0f);
            TestNodeWithAllFloatNInputVariants("math/sqrt", val, math.sqrt(val));
            TestNode("math/sqrt", -1.0f, math.NAN, ComparisonType.IsNaN);
            TestNode("math/sqrt", 0.0f, 0.0f);
        }

        [Test]
        public void TestPow()
        {
            float4 val = new float4(30.0f, 0.3f, -2f, 15.0f);
            float4 e = new float4(1.0f, 1.3f, -2f, 5.0f);
            var expected = math.pow(val, e);
            Util.Log(expected.ToString());

            TestNodeWithAllFloatNInputVariants("math/pow", val, e, expected);
            TestNode("math/pow", -1.0f, 2.0f, 1.0f, ComparisonType.Approximately);
            TestNode("math/pow", 0.0f, 1000.0f, 0.0f, ComparisonType.Approximately);
            TestNode("math/pow", 1000.0f, 0.0f, 1.0f, ComparisonType.Approximately);
            TestNode("math/pow", -0.2f, -1.2f, float.NaN, ComparisonType.IsNaN);
        }

        [Test]
        public void TestLength()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            TestNode("math/length", a.xy, math.length(a.xy));
            TestNode("math/length", a.xyz, math.length(a.xyz));
            TestNode("math/length", a, math.length(a));
        }

        [Test]
        public void TestNormalize()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            TestNode("math/normalize", a.xy, math.normalize(a.xy));
            TestNode("math/normalize", a.xyz, math.normalize(a.xyz));
            TestNode("math/normalize", a, math.normalize(a));
        }

        [Test]
        public void TestDot()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            TestNode("math/dot", a.xy, b.xy, math.dot(a.xy, b.xy));
            TestNode("math/dot", a.xyz, b.xyz, math.dot(a.xyz, b.xyz));
            TestNode("math/dot", a, b, math.dot(a, b));
        }

        [Test]
        public void TestCross()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            TestNode("math/cross", a.xyz, b.xyz, math.cross(a.xyz, b.xyz));
            TestNode("math/cross", a.xyz, a.xyz, new float3(0.0f, 0.0f, 0.0f));
        }

        [Test]
        public void TestCombine2()
        {
            TestNode("math/combine2", 1.0f, 2.0f, new float2(1.0f, 2.0f));
        }

        [Test]
        public void TestCombine3()
        {
            TestNode("math/combine3", 1.0f, 2.0f, 3.0f, new float3(1.0f, 2.0f, 3.0f));
        }

        [Test]
        public void TestCombine4()
        {
            TestNode("math/combine4", 1.0f, 2.0f, 3.0f, 4.0f, new float4(1.0f, 2.0f, 3.0f, 4.0f));
        }

        [Test]
        public void TestCombine2x2()
        {
            CombineTest("math/combine2x2", new float[] { 1.0f, 2.0f, 3.0f, 4.0f }, new float2x2(new float2(1.0f, 2.0f), new float2(3.0f, 4.0f)));
        }

        [Test]
        public void TestCombine3x3()
        {
            CombineTest("math/combine3x3", new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f }, new float3x3(new float3(1.0f, 2.0f, 3.0f), new float3(4.0f, 5.0f, 6.0f), new float3(7.0f, 8.0f, 9.0f)));
        }

        [Test]
        public void TestCombine4x4()
        {
            CombineTest("math/combine4x4", new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f }, new float4x4(new float4(1.0f, 2.0f, 3.0f, 4.0f), new float4(5.0f, 6.0f, 7.0f, 8.0f), new float4(9.0f, 10.0f, 11.0f, 12.0f), new float4(13.0f, 14.0f, 15.0f, 16.0f)));
        }

        private static void CombineTest<T>(string nodeName, float[] inputValues, T expected)
        {
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            for (int i = 0; i < inputValues.Length; i++)
            {
                inputs.Add(ConstStrings.Letters[i], new Value() { id = ConstStrings.Letters[i], property = new Property<float>(inputValues[i]) });
            }

            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            TestNode(nodeName, inputs, outputs);
        }

        [Test]
        public void TestExtract2()
        {
            var v = new float2(10.3f, -10.4f);

            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<float2>(v) });
            for (int i = 0; i < 2; i++)
            {
                outputs.Add(ConstStrings.Numbers[i], new Property<float>(v[i]));
            }

            TestNode("math/extract2", inputs, outputs);
        }

        [Test]
        public void TestExtract3()
        {
            var v = new float3(10.3f, -10.4f, 32.3f);

            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<float3>(v) });
            for (int i = 0; i < 3; i++)
            {
                outputs.Add(ConstStrings.Numbers[i], new Property<float>(v[i]));
            }

            TestNode("math/extract3", inputs, outputs);
        }

        [Test]
        public void TestExtract4()
        {
            var v = new float4(10.3f, -10.4f, 32.3f, 11.5f);
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<float4>(v) });
            for (int i = 0; i < 4; i++)
            {
                outputs.Add(ConstStrings.Numbers[i], new Property<float>(v[i]));
            }

            TestNode("math/extract4", inputs, outputs);
        }

        [Test]
        public void TestExtract2x2()
        {
            var v = new float2x2(1.0f, 2.0f, 3.0f, 4.0f);

            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<float2x2>(v) });
            for (int i = 0; i < 4; i++)
            {
                outputs.Add(ConstStrings.Numbers[i], new Property<float>(v[i / 2][i % 2]));
            }

            TestNode("math/extract2x2", inputs, outputs);
        }

        [Test]
        public void TestExtract3x3()
        {
            var v = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f);

            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<float3x3>(v) });
            for (int i = 0; i < 9; i++)
            {
                outputs.Add(ConstStrings.Numbers[i], new Property<float>(v[i / 3][i % 3]));
            }

            TestNode("math/extract3x3", inputs, outputs);
        }

        [Test]
        public void TestExtract4x4()
        {
            var v = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f);

            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<float4x4>(v) });
            for (int i = 0; i < 16; i++)
            {
                outputs.Add(ConstStrings.Numbers[i], new Property<float>(v[i / 4][i % 4]));
            }

            TestNode("math/extract4x4", inputs, outputs);
        }

        [Test]
        public void TestTranspose()
        {
            {
                var mat4 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f);
                var tmat4 = new float4x4();
                for (int i = 0; i < 16; i++)
                {
                    tmat4[i / 4][i % 4] = mat4[i % 4][i / 4];
                }
                TestNode("math/transpose", mat4, tmat4);
            }

            {
                var mat3 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f);
                var tmat3 = new float3x3();
                for (int i = 0; i < 9; i++)
                {
                    tmat3[i / 3][i % 3] = mat3[i % 3][i / 3];
                }
                TestNode("math/transpose", mat3, tmat3);
            }

            {
                var mat2 = new float2x2(1.0f, 2.0f, 3.0f, 4.0f);
                var tmat2 = new float2x2();
                for (int i = 0; i < 4; i++)
                {
                    tmat2[i / 2][i % 2] = mat2[i % 2][i / 2];
                }
                TestNode("math/transpose", mat2, tmat2);
            }
        }

        [Test]
        public void TestDeterminant()
        {
            {
                var mat4 = new float4x4(1.0f, 2.0f, 3.0f, 41.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 151.0f, 16.0f);
                TestNode("math/determinant", mat4, 20128.0f);
            }

            {
                var mat3 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 91.0f);
                TestNode("math/determinant", mat3, -246.0f);
            }

            {
                var mat2 = new float2x2(1.0f, 2.0f, 3.0f, 41.0f);
                TestNode("math/determinant", mat2, 35.0f);
            }
        }

        [Test]
        public void TestInverse()
        {

            var mat4 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 121.0f, 13.0f, 14.0f, 151.0f, 161.0f);
            var imat4 = new float4x4(-1.47672f, 0.4608f, 0.008567f, 0.007352f, 1.21262f, -0.18996f, -0.00796f, -0.0147f, 0.00492f, -0.0025f, -0.009781f, 0.007352f, 0.00917f, -0.018348f, 0.009174f, 0.0f);
            TestNode("math/inverse", mat4, imat4, ComparisonType.Approximately);

            var mat3 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 91.0f);
            var imat3 = new float3x3(-1.65447f, 0.64227f, 0.0122f, 1.30894f, -0.28455f, -0.0244f, 0.0122f, -0.0244f, 0.0122f);
            TestNode("math/inverse", mat3, imat3, ComparisonType.Approximately);

            var mat2 = new float2x2(1.0f, 2.0f, 3.0f, 41.0f);
            var imat2 = new float2x2(1.17142f, -0.05714f, -0.08571f, 0.02857f);
            TestNode("math/inverse", mat2, imat2, ComparisonType.Approximately);

        }

        [Test]
        public void TestMatMul()
        {

            var mat41 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f);
            var mat42 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 4.0f, 3.0f, 2.0f, 1.0f, 5.0f, 6.0f, 7.0f, 8.0f, 8.0f, 7.0f, 6.0f, 5.0f);
            var matres = new float4x4(56.0f, 54.0f, 52.0f, 50.0f, 128.0f, 126.0f, 124.0f, 122.0f, 200.0f, 198.0f, 196.0f, 194.0f, 272.0f, 270.0f, 268.0f, 266.0f);
            TestNode("math/matmul", mat41, mat42, matres, ComparisonType.Approximately);

            var mat31 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f);
            var mat32 = new float3x3(1.0f, 2.0f, 3.0f, 3.0f, 2.0f, 1.0f, 6.0f, 4.0f, 5.0f);
            var matres2 = new float3x3(25.0f, 18.0f, 20.0f, 55.0f, 42.0f, 47.0f, 85.0f, 66.0f, 74.0f);
            TestNode("math/matmul", mat31, mat32, matres2, ComparisonType.Approximately);


            var mat21 = new float2x2(1.0f, 2.0f, 3.0f, 4.0f);
            var mat22 = new float2x2(1.0f, 2.0f, 2.0f, 1.0f);
            var matres3 = new float2x2(5.0f, 4.0f, 11.0f, 10.0f);
            TestNode("math/matmul", mat21, mat22, matres3, ComparisonType.Approximately);
        }

        [Test]
        public void TestAnd()
        {
            TestNode("math/and", true, false, false);
            TestNode("math/and", false, false, false);
            TestNode("math/and", true, true, true);

            TestNode("math/and", 3, 8, 3 & 8);
        }

        [Test]
        public void TestOr()
        {
            TestNode("math/or", true, false, true);
            TestNode("math/or", false, false, false);
            TestNode("math/or", true, true, true);

            TestNode("math/or", 3, 8, 3 | 8);
        }

        [Test]
        public void TestXor()
        {
            TestNode("math/xor", true, false, true);
            TestNode("math/xor", false, false, false);
            TestNode("math/xor", true, true, false);

            TestNode("math/xor", 3, 8, 3 ^ 8);
        }

        [Test]
        public void TestRotate3d()
        {
            RotateTest3D("math/rotate3d", new float3(0.0f, 0.0f, -1.0f), new float3(0.0f, 1.0f, 0.0f), math.PI * 0.5f, new float3(-1.0f, 0.0f, 0.0f));
            RotateTest3D("math/rotate3d", new float3(1.0f, 0.0f, 0.0f), new float3(0.0f, 1.0f, 0.0f), math.PI, new float3(-1.0f, 0.0f, 0.0f));
        }

        private static void RotateTest3D<T, V>(string nodeName, T a, T b, V c, T expected)
        {
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<T>(a) });
            inputs.Add(ConstStrings.B, new Value() { id = ConstStrings.B, property = new Property<T>(b) });
            inputs.Add(ConstStrings.C, new Value() { id = ConstStrings.C, property = new Property<V>(c) });
            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            TestNode(nodeName, inputs, outputs, ComparisonType.Approximately);
        }

        [Test]
        public void TestRotate2d()
        {
            RotateTest2D("math/rotate2d", new float2(0.0f, 1.0f), math.PI * 0.5f, new float2(-1.0f, 0.0f));
            RotateTest2D("math/rotate2d", new float2(-1.0f, 0.0f), math.PI * 0.5f, new float2(0.0f, -1.0f));
        }

        private static void RotateTest2D<T, V>(string nodeName, T a, V b, T expected)
        {
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<T>(a) });
            inputs.Add(ConstStrings.B, new Value() { id = ConstStrings.B, property = new Property<V>(b) });
            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            TestNode(nodeName, inputs, outputs, ComparisonType.Approximately);
        }

        [Test]
        public void TestTransform()
        {
            TransformTest(new float2(10.2f, 12.1f), new float2x2(2.3f, -4.1f, 12.4f, 11.5f), new float2(-26.15f, 265.63f));
            TransformTest(new float3(10.2f, 12.1f, 16.4f), new float3x3(2.3f, -4.1f, 12.3f, 12.4f, 11.5f, 17.1f, 1.3f, -5.0f, 19.5f), new float3(175.57f, 546.07f, 272.56f));
            TransformTest(new float4(10.2f, 12.1f, 16.4f, 6.4f), new float4x4(2.3f, -4.1f, 12.3f, 8.3f, 12.4f, 11.5f, 17.1f, 83.0f, 1.3f, -5.0f, 19.5f, 14.1f, 4.4f, 19.1f, 72.3f, 18.2f), new float4(228.69f, 1077.27f, 362.8f, 1578.19f));
        }

        private static void TransformTest<T,V>(T a, V b, T expected)
        {
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            inputs.Add(ConstStrings.A, new Value() { id = ConstStrings.A, property = new Property<T>(a) });
            inputs.Add(ConstStrings.B, new Value() { id = ConstStrings.B, property = new Property<V>(b) });
            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            TestNode("math/transform", inputs, outputs, ComparisonType.Approximately);
        }

        [Test]
        public void TestCbrt()
        {
            TestNodeWithAllFloatNInputVariants("math/cbrt", new float4(11.3f, -50.3f, 33.3f, 100.1f), new float4(2.24401703f, -3.69138487f, 3.21722482f, 4.64313551f));
        }

        [Test]
        public void TestFract()
        {

            TestNodeWithAllFloatNInputVariants("math/fract", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(0.4f, 0.9f, 0.39f, 0.67f));
        }

        [Test]
        public void TestNeg()
        {
            TestNodeWithAllFloatNInputVariants("math/neg", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(-15.4f, 10.1f, -12.39f, 32.33f));
        }

        [Test]
        public void TestRad()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.radians(a);

            TestNodeWithAllFloatNInputVariants("math/rad", a, expected);
        }

        [Test]
        public void TestDeg()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.degrees(a);

            TestNodeWithAllFloatNInputVariants("math/deg", a, expected);
        }
    }
}