using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityGLTF.Interactivity.Tests
{
    public class MathNodesTests : NodeTestHelpers
    {
        protected override string _subDirectory => "Math";

        [Test]
        public void TestAsr()
        {
            QueueTest("Asr_Positive", CreateSelfContainedTestGraph("math/asr", In(17, 1), Out(8), ComparisonType.Equals));
            QueueTest("Asr_Negative", CreateSelfContainedTestGraph("math/asr", In(-19, 2), Out(-5), ComparisonType.Equals));
        }

        [Test]
        public void TestLsl()
        {
            QueueTest("Lsl_Positive", CreateSelfContainedTestGraph("math/lsl", In(25, 2), Out(100), ComparisonType.Equals));
            QueueTest("Lsl_Negative", CreateSelfContainedTestGraph("math/lsl", In(-23, 2), Out(-92), ComparisonType.Equals));
        }

        [Test]
        public void TestClz()
        {
            QueueTest("Clz_32", CreateSelfContainedTestGraph("math/clz", In(32), Out(26), ComparisonType.Equals));
            QueueTest("Clz_1", CreateSelfContainedTestGraph("math/clz", In(1), Out(31), ComparisonType.Equals));
            QueueTest("Clz_0x0fff0000", CreateSelfContainedTestGraph("math/clz", In(0x0fff0000), Out(4), ComparisonType.Equals));
        }

        [Test]
        public void TestCtz()
        {
            QueueTest("Ctz_1", CreateSelfContainedTestGraph("math/ctz", In(1), Out(0), ComparisonType.Equals));
            QueueTest("Ctz_16", CreateSelfContainedTestGraph("math/ctz", In(16), Out(4), ComparisonType.Equals));
            QueueTest("Ctz_0x0f000000", CreateSelfContainedTestGraph("math/ctz", In(0x0f000000), Out(24), ComparisonType.Equals));
        }

        [Test]
        public void TestPopcnt()
        {
            QueueTest("Popcnt_Binary", CreateSelfContainedTestGraph("math/popcnt", In(0b0000001000100000), Out(2), ComparisonType.Equals));
        }

        [Test]
        public void TestE()
        {
            QueueTest("E_Constant", CreateSelfContainedTestGraph("math/e", new(), Out(math.E), ComparisonType.Equals));
        }

        [Test]
        public void TestPI()
        {
            QueueTest("PI_Constant", CreateSelfContainedTestGraph("math/pi", new(), Out(math.PI), ComparisonType.Equals));
        }

        [Test]
        public void TestInf()
        {
            QueueTest("Infinity_Constant", CreateSelfContainedTestGraph("math/inf", new(), Out(math.INFINITY), ComparisonType.IsInfinity));
        }

        [Test]
        public void TestNAN()
        {
            QueueTest("NaN_Constant", CreateSelfContainedTestGraph("math/nan", new(), Out(math.NAN), ComparisonType.IsNaN));
        }


        [Test]
        public void TestAbs()
        {
            QueueTest("Abs_Negative", CreateSelfContainedTestGraph("math/abs", In(-2), Out(2), ComparisonType.Equals));
            QueueTest("Abs_Positive", CreateSelfContainedTestGraph("math/abs", In(9), Out(9), ComparisonType.Equals));

            TestNodeWithAllFloatNInputVariants("Abs", "math/abs", new float4(-2f, 2f, -9.15f, 0f), new float4(2f, 2f, 9.15f, 0f));
        }

        [Test]
        public void TestSign()
        {
            QueueTest("Sign_Float_Positive", CreateSelfContainedTestGraph("math/sign", In(32.0f), Out(1.0f), ComparisonType.Equals));
            QueueTest("Sign_Float_Negative", CreateSelfContainedTestGraph("math/sign", In(-12.0f), Out(-1.0f), ComparisonType.Equals));
            QueueTest("Sign_Float_Zero", CreateSelfContainedTestGraph("math/sign", In(0.0f), Out(0.0f), ComparisonType.Equals));

            QueueTest("Sign_Int_Positive", CreateSelfContainedTestGraph("math/sign", In(32), Out(1), ComparisonType.Equals));
            QueueTest("Sign_Int_Negative", CreateSelfContainedTestGraph("math/sign", In(-12), Out(-1), ComparisonType.Equals));
            QueueTest("Sign_Int_Zero", CreateSelfContainedTestGraph("math/sign", In(0), Out(0), ComparisonType.Equals));

            TestNodeWithAllFloatNInputVariants("Sign", "math/sign", new float4(32.0f, -12.0f, 0.0f, 5.0f), new float4(1.0f, -1.0f, 0.0f, 1.0f));
        }

        [Test]
        public void TestFloor()
        {
            TestNodeWithAllFloatNInputVariants("Floor", "math/floor", new float4(3.87f, 3.14f, -3.14f, -3.87f), new float4(3f, 3f, -4f, -4f));
        }

        [Test]
        public void TestTrunc()
        {
            TestNodeWithAllFloatNInputVariants("Trunc", "math/trunc", new float4(3.87f, 3.14f, -3.14f, -3.87f), new float4(3f, 3f, -3f, -3f));
        }

        [Test]
        public void TestCeil()
        {
            TestNodeWithAllFloatNInputVariants("Ceil", "math/ceil", new float4(3.87f, 3.14f, -3.14f, -3.87f), new float4(4f, 4f, -3f, -3f));
        }

        [Test]
        public void TestAdd()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(-34.0f, 22f, -11f, 70.0f);

            var expected = a + b;

            TestNodeWithAllFloatNInputVariants("Add", "math/add", a, b, expected);
            QueueTest("Add_Positive", CreateSelfContainedTestGraph("math/add", In(5, 15), Out(20), ComparisonType.Equals));
            QueueTest("Add_Negative", CreateSelfContainedTestGraph("math/add", In(5, -15), Out(-10), ComparisonType.Equals));
            QueueTest("Add_ZeroOperand", CreateSelfContainedTestGraph("math/add", In(5, 0), Out(5), ComparisonType.Equals));
            QueueTest("Add_BothZero", CreateSelfContainedTestGraph("math/add", In(0, 0), Out(0), ComparisonType.Equals));
        }

        [Test]
        public void TestSub()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(-34.0f, 22f, -11f, 70.0f);

            var expected = a - b;

            TestNodeWithAllFloatNInputVariants("Sub", "math/sub", a, b, expected);
            QueueTest("Sub_Positive", CreateSelfContainedTestGraph("math/sub", In(5, 15), Out(-10), ComparisonType.Equals));
            QueueTest("Sub_Negative", CreateSelfContainedTestGraph("math/sub", In(5, -15), Out(20), ComparisonType.Equals));
            QueueTest("Sub_ZeroOperand", CreateSelfContainedTestGraph("math/sub", In(5, 0), Out(5), ComparisonType.Equals));
            QueueTest("Sub_BothZero", CreateSelfContainedTestGraph("math/sub", In(0, 0), Out(0), ComparisonType.Equals));
        }

        [Test]
        public void TestMul()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(-34.0f, 22f, -11f, 70.0f);

            var expected = a * b;

            TestNodeWithAllFloatNInputVariants("Mul", "math/mul", a, b, expected);
            QueueTest("Mul_Positive", CreateSelfContainedTestGraph("math/mul", In(1, 15), Out(15), ComparisonType.Equals));
            QueueTest("Mul_ZeroOperand_Negative", CreateSelfContainedTestGraph("math/mul", In(0, -15), Out(0), ComparisonType.Equals));
            QueueTest("Mul_ZeroOperand_Positive", CreateSelfContainedTestGraph("math/mul", In(5, 0), Out(0), ComparisonType.Equals));
            QueueTest("Mul_Positive", CreateSelfContainedTestGraph("math/mul", In(5, 5), Out(25), ComparisonType.Equals));
            QueueTest("Mul_Negative", CreateSelfContainedTestGraph("math/mul", In(5, -5), Out(-25), ComparisonType.Equals));
        }

        [Test]
        public void TestDiv()
        {
            var a = new float4(5f, 5f, 12.4f, -12.4f);
            var b = new float4(1f, 12f, -55f, -12.4f);
            var expected = a / b;
            TestNodeWithAllFloatNInputVariants("Div", "math/div", a, b, expected);
            QueueTest("Div_ByZero", CreateSelfContainedTestGraph("math/div", In(5f, 0f), Out(float.PositiveInfinity), ComparisonType.IsInfinity));
            QueueTest("Div_ByPositiveInfinity", CreateSelfContainedTestGraph("math/div", In(5f, float.PositiveInfinity), Out(0f), ComparisonType.Equals));
        }

        [Test]
        public void TestRem()
        {
            var a = new float4(5f, 5f, 12.4f, -12.4f);
            var b = new float4(1f, 12f, -55f, -12.4f);
            var expected = a % b;

            TestNodeWithAllFloatNInputVariants("Rem_DivideByZero", "math/rem", 5.0f, 0.0f, math.NAN, ComparisonType.IsNaN);
            TestNodeWithAllFloatNInputVariants("Rem_DivideByInfinity", "math/rem", 5.0f, math.INFINITY, 5.0f, ComparisonType.Equals);
            TestNodeWithAllFloatNInputVariants("Rem", "math/rem", a, b, expected);
            QueueTest("Rem_Int_Positive", CreateSelfContainedTestGraph("math/rem", In(5, 4), Out(1), ComparisonType.Equals));
            QueueTest("Rem_Int_Equal", CreateSelfContainedTestGraph("math/rem", In(5, 5), Out(0), ComparisonType.Equals));
        }

        [Test]
        public void TestMin()
        {
            TestNodeWithAllFloatNInputVariants("Min", "math/min", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(3.0f, 0.0f, -32.0f, 7.0f));

            QueueTest("Min_Positive", CreateSelfContainedTestGraph("math/min", In(100, 10), Out(10), ComparisonType.Equals));
        }

        [Test]
        public void TestMax()
        {
            TestNodeWithAllFloatNInputVariants("Max", "math/max", new float4(12.0f, 16.0f, -32.0f, 7.0f), new float4(3.0f, 0.0f, 14.0f, 14.0f), new float4(12.0f, 16.0f, 14.0f, 14.0f));

            QueueTest("Max_Positive", CreateSelfContainedTestGraph("math/max", In(100, 10), Out(100), ComparisonType.Equals));
        }

        [Test]
        public void TestClamp()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);
            var c = new float4(92.0f, 43.0f, 59.0f, 90.0f);

            var expected = math.clamp(a, b, c);

            TestNodeWithAllFloatNInputVariants("Clamp", "math/clamp", a, b, c, expected);
        }

        [Test]
        public void TestSaturate()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            TestNodeWithAllFloatNInputVariants("Saturate", "math/saturate", a, math.saturate(a));
        }

        [Test]
        public void TestMix()
        {
            TestNodeWithAllFloatNInputVariants("Mix", "math/mix", new float4(1.0f, 2.0f, 3.0f, 4.0f), new float4(9.0f, 10.0f, 11.0f, 12.0f), new float4(1.0f, 0.25f, 0.5f, 0.0f), new float4(9.0f, 4.0f, 7.0f, 4.0f));
        }

        [Test]
        public void TestEq()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            QueueTest("Eq_X_Comparison_False", CreateSelfContainedTestGraph("math/eq", In(a.x, b.x), Out(false), ComparisonType.Equals));
            QueueTest("Eq_XY_Comparison_False", CreateSelfContainedTestGraph("math/eq", In(a.xy, b.xy), Out(false), ComparisonType.Equals));
            QueueTest("Eq_XYZ_Comparison_False", CreateSelfContainedTestGraph("math/eq", In(a.xyz, b.xyz), Out(false), ComparisonType.Equals));
            QueueTest("Eq_Full_Comparison_False", CreateSelfContainedTestGraph("math/eq", In(a, b), Out(false), ComparisonType.Equals));

            QueueTest("Eq_X_Comparison_True", CreateSelfContainedTestGraph("math/eq", In(a.x, a.x), Out(true), ComparisonType.Equals));
            QueueTest("Eq_XY_Comparison_True", CreateSelfContainedTestGraph("math/eq", In(a.xy, a.xy), Out(true), ComparisonType.Equals));
            QueueTest("Eq_XYZ_Comparison_True", CreateSelfContainedTestGraph("math/eq", In(a.xyz, a.xyz), Out(true), ComparisonType.Equals));
            QueueTest("Eq_Full_Comparison_True", CreateSelfContainedTestGraph("math/eq", In(a, a), Out(true), ComparisonType.Equals));

            QueueTest("Eq_Int_Comparison_False", CreateSelfContainedTestGraph("math/eq", In(1, 2), Out(false), ComparisonType.Equals));
            QueueTest("Eq_Int_Comparison_True", CreateSelfContainedTestGraph("math/eq", In(1, 1), Out(true), ComparisonType.Equals));

            QueueTest("Eq_NegativeComparison_False", CreateSelfContainedTestGraph("math/eq", In(-2, 2), Out(false), ComparisonType.Equals));
            QueueTest("Eq_NegativeComparison_True", CreateSelfContainedTestGraph("math/eq", In(-2, -2), Out(true), ComparisonType.Equals));
        }

        [Test]
        public void TestLT()
        {
            QueueTest("LT_Float_Positive", CreateSelfContainedTestGraph("math/lt", In(10.0f, 20.0f), Out(true), ComparisonType.Equals));
            QueueTest("LT_Float_Negative", CreateSelfContainedTestGraph("math/lt", In(40.0f, 20.0f), Out(false), ComparisonType.Equals));

            QueueTest("LT_Int_Positive", CreateSelfContainedTestGraph("math/lt", In(10, 20), Out(true), ComparisonType.Equals));
            QueueTest("LT_Int_Negative", CreateSelfContainedTestGraph("math/lt", In(40, 20), Out(false), ComparisonType.Equals));
        }

        [Test]
        public void TestLE()
        {
            QueueTest("LE_Float_Less", CreateSelfContainedTestGraph("math/le", In(10.0f, 20.0f), Out(true), ComparisonType.Equals));
            QueueTest("LE_Float_Equal", CreateSelfContainedTestGraph("math/le", In(10.0f, 10.0f), Out(true), ComparisonType.Equals));
            QueueTest("LE_Float_Greater", CreateSelfContainedTestGraph("math/le", In(40.0f, 20.0f), Out(false), ComparisonType.Equals));

            QueueTest("LE_Int_Less", CreateSelfContainedTestGraph("math/le", In(10, 20), Out(true), ComparisonType.Equals));
            QueueTest("LE_Int_Equal", CreateSelfContainedTestGraph("math/le", In(10, 10), Out(true), ComparisonType.Equals));
            QueueTest("LE_Int_Greater", CreateSelfContainedTestGraph("math/le", In(40, 20), Out(false), ComparisonType.Equals));
        }

        [Test]
        public void TestGT()
        {
            QueueTest("GT_Float_Less", CreateSelfContainedTestGraph("math/gt", In(10.0f, 20.0f), Out(false), ComparisonType.Equals));
            QueueTest("GT_Float_Greater", CreateSelfContainedTestGraph("math/gt", In(40.0f, 20.0f), Out(true), ComparisonType.Equals));

            QueueTest("GT_Int_Less", CreateSelfContainedTestGraph("math/gt", In(10, 20), Out(false), ComparisonType.Equals));
            QueueTest("GT_Int_Greater", CreateSelfContainedTestGraph("math/gt", In(40, 20), Out(true), ComparisonType.Equals));
        }

        [Test]
        public void TestGE()
        {
            QueueTest("GE_Float_Less", CreateSelfContainedTestGraph("math/ge", In(10.0f, 20.0f), Out(false), ComparisonType.Equals));
            QueueTest("GE_Float_Equal", CreateSelfContainedTestGraph("math/ge", In(10.0f, 10.0f), Out(true), ComparisonType.Equals));
            QueueTest("GE_Float_Greater", CreateSelfContainedTestGraph("math/ge", In(40.0f, 20.0f), Out(true), ComparisonType.Equals));

            QueueTest("GE_Int_Less", CreateSelfContainedTestGraph("math/ge", In(10, 20), Out(false), ComparisonType.Equals));
            QueueTest("GE_Int_Equal", CreateSelfContainedTestGraph("math/ge", In(10, 10), Out(true), ComparisonType.Equals));
            QueueTest("GE_Int_Greater", CreateSelfContainedTestGraph("math/ge", In(40, 20), Out(true), ComparisonType.Equals));
        }

        [Test]
        public void TestIsNan()
        {
            QueueTest("IsNan_True", CreateSelfContainedTestGraph("math/isnan", In((float)math.acos(-2.0)), Out(true), ComparisonType.Equals));
            QueueTest("IsNan_False", CreateSelfContainedTestGraph("math/isnan", In(10.0f), Out(false), ComparisonType.Equals));
        }

        [Test]
        public void TestIsInf()
        {
            QueueTest("IsInf_True", CreateSelfContainedTestGraph("math/isinf", In(10.0f / 0.0f), Out(true), ComparisonType.Equals));
            QueueTest("IsInf_False", CreateSelfContainedTestGraph("math/isinf", In(10.0f), Out(false), ComparisonType.Equals));
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

            CreateSelfContainedTestGraph("math/select", inputs, outputs, ComparisonType.Equals);
        }

        [Test]
        public void TestSin()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.sin(a);

            TestNodeWithAllFloatNInputVariants("Sin", "math/sin", a, expected);
            QueueTest("Sin_Zero", CreateSelfContainedTestGraph("math/sin", In(0.0f), Out(0.0f), ComparisonType.Approximately));
            QueueTest("Sin_PIOver2", CreateSelfContainedTestGraph("math/sin", In((float)(math.PI / 2.0)), Out(1.0f), ComparisonType.Approximately));
        }

        [Test]
        public void TestCos()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.cos(a);

            TestNodeWithAllFloatNInputVariants("Cos", "math/cos", a, expected);
            QueueTest("Cos_Zero", CreateSelfContainedTestGraph("math/cos", In(0.0f), Out(1.0f), ComparisonType.Approximately));
            QueueTest("Cos_PIOver2", CreateSelfContainedTestGraph("math/cos", In((float)(math.PI / 2.0)), Out(0.0f), ComparisonType.Approximately));
        }

        [Test]
        public void TestTan()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.tan(a);

            TestNodeWithAllFloatNInputVariants("Tan", "math/tan", a, expected);
            // TestNode("math/tan", math.PI * 0.5f, math.INFINITY); // it fails because of PI not precise
        }

        [Test]
        public void TestAsin()
        {
            var a = new float4(-1f, 1f, 0f, 0.4f);
            var expected = new float4(-math.PI / 2f, math.PI / 2f, 0f, 0.411516f);

            TestNodeWithAllFloatNInputVariants("Asin", "math/asin", a, expected);

            QueueTest("Asin_OutOfRange", CreateSelfContainedTestGraph("math/asin", In(1000.0f), Out(math.NAN), ComparisonType.IsNaN));
        }

        [Test]
        public void TestAcos()
        {
            var a = new float4(0f, 0.999f, -0.9999f, 0.1f);
            var expected = math.acos(a);

            TestNodeWithAllFloatNInputVariants("Acos", "math/acos", a, expected);

            //TestNode("math/acos", 1000.0f, math.NAN, ComparisonType.IsNaN);
            //TestNode("math/acos", 0.0f, math.PI * 0.5f, ComparisonType.Approximately);
        }

        [Test]
        public void TestAtan()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.atan(a);

            TestNodeWithAllFloatNInputVariants("Atan", "math/atan", a, expected);

            QueueTest("Atan_Positive", CreateSelfContainedTestGraph("math/atan", In(1.0f), Out(math.PI / 4.0f), ComparisonType.Equals));
            QueueTest("Atan_Zero", CreateSelfContainedTestGraph("math/atan", In(0.0f), Out(0.0f), ComparisonType.Equals));
        }

        [Test]
        public void TestAtan2()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            var expected = math.atan2(a, b);

            TestNodeWithAllFloatNInputVariants("Atan2", "math/atan2", a, b, expected);

            QueueTest("Atan2_Positive", CreateSelfContainedTestGraph("math/atan2", In(1.0f, 1.0f), Out(math.PI / 4.0f), ComparisonType.Equals));
            QueueTest("Atan2_SecondQuadrant", CreateSelfContainedTestGraph("math/atan2", In(1.0f, 0.0f), Out(math.PI / 2.0f), ComparisonType.Equals));
        }

        [Test]
        public void TestSinH()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.sinh(a);

            TestNodeWithAllFloatNInputVariants("Sinh", "math/sinh", a, expected);
            QueueTest("Sinh_Zero", CreateSelfContainedTestGraph("math/sinh", In(0.0f), Out(0.0f), ComparisonType.Equals));
        }

        [Test]
        public void TestCosH()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.cosh(a);

            TestNodeWithAllFloatNInputVariants("Cosh", "math/cosh", a, expected);
            QueueTest("Cosh_Zero", CreateSelfContainedTestGraph("math/cosh", In(0.0f), Out(1.0f), ComparisonType.Equals));
        }

        [Test]
        public void TestTanH()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.tanh(a);

            TestNodeWithAllFloatNInputVariants("Tanh", "math/tanh", a, expected);
            QueueTest("Tanh_PositiveInfinity", CreateSelfContainedTestGraph("math/tanh", In(math.INFINITY), Out(1.0f), ComparisonType.Equals));
            QueueTest("Tanh_NegativeInfinity", CreateSelfContainedTestGraph("math/tanh", In(-math.INFINITY), Out(-1.0f), ComparisonType.Equals));
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

            TestNodeWithAllFloatNInputVariants("Asinh", "math/asinh", a, expected);
        }

        [Test]
        public void TestACosH()
        {
            float4 val = new float4(1, 10.0f, 100.0f, 1000.0f);
            TestNodeWithAllFloatNInputVariants("Acosh", "math/acosh", val, acosh(val), ComparisonType.Approximately);
            QueueTest("Acosh_One", CreateSelfContainedTestGraph("math/acosh", In(1.0f), Out(0.0f), ComparisonType.Equals));
            QueueTest("Acosh_LessThanOne", CreateSelfContainedTestGraph("math/acosh", In(0.5f), Out(float.NaN), ComparisonType.IsNaN));
        }

        [Test]
        public void TestATanH()
        {
            float4 val = new float4(-0.99f, -0.3f, 0.3f, 0.99f);
            TestNodeWithAllFloatNInputVariants("Atanh", "math/atanh", val, atanh(val));
            QueueTest("Atanh_PositiveOne", CreateSelfContainedTestGraph("math/atanh", In(1.0f), Out(math.INFINITY), ComparisonType.IsInfinity));
            QueueTest("Atanh_NegativeOne", CreateSelfContainedTestGraph("math/atanh", In(-1.0f), Out(-math.INFINITY), ComparisonType.IsInfinity));
            QueueTest("Atanh_PositiveGreaterThanOne", CreateSelfContainedTestGraph("math/atanh", In(1.1f), Out(math.NAN), ComparisonType.IsNaN));
            QueueTest("Atanh_NegativeGreaterThanOne", CreateSelfContainedTestGraph("math/atanh", In(-1.1f), Out(-math.NAN), ComparisonType.IsNaN));
        }

        [Test]
        public void TestExp()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.exp(a);

            TestNodeWithAllFloatNInputVariants("Exp", "math/exp", a, expected);
        }

        [Test]
        public void TestLog()
        {
            float4 val = new float4(30.0f, 0.3f, 1f, 15.0f);
            QueueTest("Log_PositiveValue", CreateSelfContainedTestGraph("math/log", In(val), Out(math.log(val)), ComparisonType.Approximately));
            QueueTest("Log_NegativeValue", CreateSelfContainedTestGraph("math/log", In(-1.0f), Out(math.NAN), ComparisonType.IsNaN));
            QueueTest("Log_Zero", CreateSelfContainedTestGraph("math/log", In(0.0f), Out(-math.INFINITY), ComparisonType.IsInfinity));
        }

        [Test]
        public void TestLog2()
        {
            float4 val = new float4(30.0f, 0.3f, 1f, 15.0f);
            TestNodeWithAllFloatNInputVariants("Log2", "math/log2", val, math.log2(val));
            QueueTest("Log2_NegativeValue", CreateSelfContainedTestGraph("math/log2", In(-1.0f), Out(math.NAN), ComparisonType.IsNaN));
            QueueTest("Log2_Zero", CreateSelfContainedTestGraph("math/log2", In(0.0f), Out(-math.INFINITY), ComparisonType.IsInfinity));
        }

        [Test]
        public void TestLog10()
        {
            float4 val = new float4(30.0f, 0.3f, 1f, 15.0f);
            TestNodeWithAllFloatNInputVariants("Log10", "math/log10", val, math.log10(val));
            QueueTest("Log10_NegativeValue", CreateSelfContainedTestGraph("math/log10", In(-1.0f), Out(math.NAN), ComparisonType.IsNaN));
            QueueTest("Log10_Zero", CreateSelfContainedTestGraph("math/log10", In(0.0f), Out(-math.INFINITY), ComparisonType.IsInfinity));
        }

        [Test]
        public void TestSqrt()
        {
            float4 val = new float4(30.0f, 0.3f, 0.2f, 15.0f);
            TestNodeWithAllFloatNInputVariants("Sqrt", "math/sqrt", val, math.sqrt(val));
            QueueTest("Sqrt_NegativeValue", CreateSelfContainedTestGraph("math/sqrt", In(-1.0f), Out(math.NAN), ComparisonType.IsNaN));
            QueueTest("Sqrt_Zero", CreateSelfContainedTestGraph("math/sqrt", In(0.0f), Out(0.0f), ComparisonType.Equals));
        }

        [Test]
        public void TestPow()
        {
            float4 val = new float4(30.0f, 0.3f, -2f, 15.0f);
            float4 e = new float4(1.0f, 1.3f, -2f, 5.0f);
            var expected = math.pow(val, e);
            Util.Log(expected.ToString());

            TestNodeWithAllFloatNInputVariants("Pow", "math/pow", val, e, expected);
            QueueTest("Pow_NegativeBase_PositiveExponent", CreateSelfContainedTestGraph("math/pow", In(-1.0f, 2.0f), Out(1.0f), ComparisonType.Approximately));
            QueueTest("Pow_Zero_PositiveExponent", CreateSelfContainedTestGraph("math/pow", In(0.0f, 1000.0f), Out(0.0f), ComparisonType.Approximately));
            QueueTest("Pow_PositiveBase_ZeroExponent", CreateSelfContainedTestGraph("math/pow", In(1000.0f, 0.0f), Out(1.0f), ComparisonType.Approximately));
            QueueTest("Pow_NegativeBase_NonIntegerExponent", CreateSelfContainedTestGraph("math/pow", In(-0.2f, -1.2f), Out(float.NaN), ComparisonType.IsNaN));
        }

        [Test]
        public void TestLength()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            QueueTest("Length_XY", CreateSelfContainedTestGraph("math/length", In(a.xy), Out(math.length(a.xy)), ComparisonType.Equals));
            QueueTest("Length_XYZ", CreateSelfContainedTestGraph("math/length", In(a.xyz), Out(math.length(a.xyz)), ComparisonType.Equals));
            QueueTest("Length_XYZW", CreateSelfContainedTestGraph("math/length", In(a), Out(math.length(a)), ComparisonType.Equals));
        }

        [Test]
        public void TestNormalize()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            QueueTest("Normalize_XY", CreateSelfContainedTestGraph("math/normalize", In(a.xy), Out(math.normalize(a.xy)), ComparisonType.Equals));
            QueueTest("Normalize_XYZ", CreateSelfContainedTestGraph("math/normalize", In(a.xyz), Out(math.normalize(a.xyz)), ComparisonType.Equals));
            QueueTest("Normalize_XYZW", CreateSelfContainedTestGraph("math/normalize", In(a), Out(math.normalize(a)), ComparisonType.Equals));
        }

        [Test]
        public void TestDot()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            QueueTest("Dot_XY", CreateSelfContainedTestGraph("math/dot", In(a.xy, b.xy), Out(math.dot(a.xy, b.xy)), ComparisonType.Equals));
            QueueTest("Dot_XYZ", CreateSelfContainedTestGraph("math/dot", In(a.xyz, b.xyz), Out(math.dot(a.xyz, b.xyz)), ComparisonType.Equals));
            QueueTest("Dot_XYZW", CreateSelfContainedTestGraph("math/dot", In(a, b), Out(math.dot(a, b)), ComparisonType.Equals));
        }

        [Test]
        public void TestCross()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var b = new float4(30.0f, 40.0f, 50.0f, 60.0f);

            QueueTest("Cross_XYZ", CreateSelfContainedTestGraph("math/cross", In(a.xyz, b.xyz), Out(math.cross(a.xyz, b.xyz)), ComparisonType.Equals));
            QueueTest("Cross_XYZ_Self", CreateSelfContainedTestGraph("math/cross", In(a.xyz, a.xyz), Out(new float3(0.0f, 0.0f, 0.0f)), ComparisonType.Equals));
        }

        [Test]
        public void TestCombine2()
        {
            QueueTest("Combine2", CreateSelfContainedTestGraph("math/combine2", In(1.0f, 2.0f), Out(new float2(1.0f, 2.0f)), ComparisonType.Equals));
        }

        [Test]
        public void TestCombine3()
        {
            QueueTest("Combine3", CreateSelfContainedTestGraph("math/combine3", In(1.0f, 2.0f, 3.0f), Out(new float3(1.0f, 2.0f, 3.0f)), ComparisonType.Equals));
        }

        [Test]
        public void TestCombine4()
        {
            QueueTest("Combine4", CreateSelfContainedTestGraph("math/combine4", In(1.0f, 2.0f, 3.0f, 4.0f), Out(new float4(1.0f, 2.0f, 3.0f, 4.0f)), ComparisonType.Equals));
        }

        [Test]
        public void TestCombine2x2()
        {
            CombineTest("Combine2x2", "math/combine2x2", new float[] { 1.0f, 2.0f, 3.0f, 4.0f }, new float2x2(new float2(1.0f, 2.0f), new float2(3.0f, 4.0f)));
        }

        [Test]
        public void TestCombine3x3()
        {
            CombineTest("Combine3x3", "math/combine3x3", new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f }, new float3x3(new float3(1.0f, 2.0f, 3.0f), new float3(4.0f, 5.0f, 6.0f), new float3(7.0f, 8.0f, 9.0f)));
        }

        [Test]
        public void TestCombine4x4()
        {
            CombineTest("Combine4x4", "math/combine4x4", new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f }, new float4x4(new float4(1.0f, 2.0f, 3.0f, 4.0f), new float4(5.0f, 6.0f, 7.0f, 8.0f), new float4(9.0f, 10.0f, 11.0f, 12.0f), new float4(13.0f, 14.0f, 15.0f, 16.0f)));
        }

        private static void CombineTest<T>(string testName, string nodeName, float[] inputValues, T expected)
        {
            var inputs = new Dictionary<string, Value>();
            var outputs = new Dictionary<string, IProperty>();

            for (int i = 0; i < inputValues.Length; i++)
            {
                inputs.Add(ConstStrings.Letters[i], new Value() { id = ConstStrings.Letters[i], property = new Property<float>(inputValues[i]) });
            }

            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            QueueTest(testName, CreateSelfContainedTestGraph(nodeName, inputs, outputs, ComparisonType.Equals));
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

            QueueTest("Extract2", CreateSelfContainedTestGraph("math/extract2", inputs, outputs, ComparisonType.Equals));
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

            QueueTest("Extract3", CreateSelfContainedTestGraph("math/extract3", inputs, outputs, ComparisonType.Equals));
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

            QueueTest("Extract4", CreateSelfContainedTestGraph("math/extract4", inputs, outputs, ComparisonType.Equals));
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

            QueueTest("Extract2x2", CreateSelfContainedTestGraph("math/extract2x2", inputs, outputs, ComparisonType.Equals));
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

            QueueTest("Extract3x3", CreateSelfContainedTestGraph("math/extract3x3", inputs, outputs, ComparisonType.Equals));
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

            QueueTest("Extract4x4", CreateSelfContainedTestGraph("math/extract4x4", inputs, outputs, ComparisonType.Equals));
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
                QueueTest("Transpose_4x4", CreateSelfContainedTestGraph("math/transpose", In(mat4), Out(tmat4), ComparisonType.Equals));
            }

            {
                var mat3 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f);
                var tmat3 = new float3x3();
                for (int i = 0; i < 9; i++)
                {
                    tmat3[i / 3][i % 3] = mat3[i % 3][i / 3];
                }
                QueueTest("Transpose_3x3", CreateSelfContainedTestGraph("math/transpose", In(mat3), Out(tmat3), ComparisonType.Equals));
            }

            {
                var mat2 = new float2x2(1.0f, 2.0f, 3.0f, 4.0f);
                var tmat2 = new float2x2();
                for (int i = 0; i < 4; i++)
                {
                    tmat2[i / 2][i % 2] = mat2[i % 2][i / 2];
                }
                QueueTest("Transpose_2x2", CreateSelfContainedTestGraph("math/transpose", In(mat2), Out(tmat2), ComparisonType.Equals));
            }
        }

        [Test]
        public void TestDeterminant()
        {
            // 4x4 Matrix Determinant
            var mat4 = new float4x4(1.0f, 2.0f, 3.0f, 41.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 151.0f, 16.0f);
            QueueTest("Determinant_4x4", CreateSelfContainedTestGraph("math/determinant", In(mat4), Out(20128.0f), ComparisonType.Equals));

            // 3x3 Matrix Determinant
            var mat3 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 91.0f);
            QueueTest("Determinant_3x3", CreateSelfContainedTestGraph("math/determinant", In(mat3), Out(-246.0f), ComparisonType.Equals));

            // 2x2 Matrix Determinant
            var mat2 = new float2x2(1.0f, 2.0f, 3.0f, 41.0f);
            QueueTest("Determinant_2x2", CreateSelfContainedTestGraph("math/determinant", In(mat2), Out(35.0f), ComparisonType.Equals));
        }

        [Test]
        public void TestInverse()
        {
            // 4x4 Matrix Inverse
            var mat4 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 121.0f, 13.0f, 14.0f, 151.0f, 161.0f);
            var imat4 = new float4x4(-1.47672f, 0.4608f, 0.008567f, 0.007352f, 1.21262f, -0.18996f, -0.00796f, -0.0147f, 0.00492f, -0.0025f, -0.009781f, 0.007352f, 0.00917f, -0.018348f, 0.009174f, 0.0f);
            QueueTest("Inverse_4x4", CreateSelfContainedTestGraph("math/inverse", In(mat4), Out(imat4), ComparisonType.Approximately));

            // 3x3 Matrix Inverse
            var mat3 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 91.0f);
            var imat3 = new float3x3(-1.65447f, 0.64227f, 0.0122f, 1.30894f, -0.28455f, -0.0244f, 0.0122f, -0.0244f, 0.0122f);
            QueueTest("Inverse_3x3", CreateSelfContainedTestGraph("math/inverse", In(mat3), Out(imat3), ComparisonType.Approximately));

            // 2x2 Matrix Inverse
            var mat2 = new float2x2(1.0f, 2.0f, 3.0f, 41.0f);
            var imat2 = new float2x2(1.17142f, -0.05714f, -0.08571f, 0.02857f);
            QueueTest("Inverse_2x2", CreateSelfContainedTestGraph("math/inverse", In(mat2), Out(imat2), ComparisonType.Approximately));
        }

        [Test]
        public void TestMatMul()
        {
            // 4x4 Matrix Multiplication
            var mat41 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f, 16.0f);
            var mat42 = new float4x4(1.0f, 2.0f, 3.0f, 4.0f, 4.0f, 3.0f, 2.0f, 1.0f, 5.0f, 6.0f, 7.0f, 8.0f, 8.0f, 7.0f, 6.0f, 5.0f);
            var matres = new float4x4(56.0f, 54.0f, 52.0f, 50.0f, 128.0f, 126.0f, 124.0f, 122.0f, 200.0f, 198.0f, 196.0f, 194.0f, 272.0f, 270.0f, 268.0f, 266.0f);
            QueueTest("MatMul_4x4", CreateSelfContainedTestGraph("math/matmul", In(mat41, mat42), Out(matres), ComparisonType.Approximately));

            // 3x3 Matrix Multiplication
            var mat31 = new float3x3(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f);
            var mat32 = new float3x3(1.0f, 2.0f, 3.0f, 3.0f, 2.0f, 1.0f, 6.0f, 4.0f, 5.0f);
            var matres2 = new float3x3(25.0f, 18.0f, 20.0f, 55.0f, 42.0f, 47.0f, 85.0f, 66.0f, 74.0f);
            QueueTest("MatMul_3x3", CreateSelfContainedTestGraph("math/matmul", In(mat31, mat32), Out(matres2), ComparisonType.Approximately));

            // 2x2 Matrix Multiplication
            var mat21 = new float2x2(1.0f, 2.0f, 3.0f, 4.0f);
            var mat22 = new float2x2(1.0f, 2.0f, 2.0f, 1.0f);
            var matres3 = new float2x2(5.0f, 4.0f, 11.0f, 10.0f);
            QueueTest("MatMul_2x2", CreateSelfContainedTestGraph("math/matmul", In(mat21, mat22), Out(matres3), ComparisonType.Approximately));
        }

        [Test]
        public void TestAnd()
        {
            QueueTest("And_Boolean_False", CreateSelfContainedTestGraph("math/and", In(true, false), Out(false), ComparisonType.Equals));
            QueueTest("And_Boolean_False", CreateSelfContainedTestGraph("math/and", In(false, false), Out(false), ComparisonType.Equals));
            QueueTest("And_Boolean_True", CreateSelfContainedTestGraph("math/and", In(true, true), Out(true), ComparisonType.Equals));

            QueueTest("And_Integer", CreateSelfContainedTestGraph("math/and", In(3, 8), Out(3 & 8), ComparisonType.Equals));
        }

        [Test]
        public void TestOr()
        {
            QueueTest("Or_Boolean_True", CreateSelfContainedTestGraph("math/or", In(true, false), Out(true), ComparisonType.Equals));
            QueueTest("Or_Boolean_False", CreateSelfContainedTestGraph("math/or", In(false, false), Out(false), ComparisonType.Equals));
            QueueTest("Or_Boolean_True", CreateSelfContainedTestGraph("math/or", In(true, true), Out(true), ComparisonType.Equals));

            QueueTest("Or_Integer", CreateSelfContainedTestGraph("math/or", In(3, 8), Out(3 | 8), ComparisonType.Equals));
        }

        [Test]
        public void TestXor()
        {
            QueueTest("Xor_Boolean_True", CreateSelfContainedTestGraph("math/xor", In(true, false), Out(true), ComparisonType.Equals));
            QueueTest("Xor_Boolean_False", CreateSelfContainedTestGraph("math/xor", In(false, false), Out(false), ComparisonType.Equals));
            QueueTest("Xor_Boolean_False", CreateSelfContainedTestGraph("math/xor", In(true, true), Out(false), ComparisonType.Equals));

            QueueTest("Xor_Integer", CreateSelfContainedTestGraph("math/xor", In(3, 8), Out(3 ^ 8), ComparisonType.Equals));
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

            CreateSelfContainedTestGraph(nodeName, inputs, outputs, ComparisonType.Approximately);
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

            CreateSelfContainedTestGraph(nodeName, inputs, outputs, ComparisonType.Approximately);
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

            CreateSelfContainedTestGraph("math/transform", inputs, outputs, ComparisonType.Approximately);
        }

        [Test]
        public void TestCbrt()
        {
            TestNodeWithAllFloatNInputVariants("Cbrt", "math/cbrt", new float4(11.3f, -50.3f, 33.3f, 100.1f), new float4(2.24401703f, -3.69138487f, 3.21722482f, 4.64313551f));
        }

        [Test]
        public void TestFract()
        {

            TestNodeWithAllFloatNInputVariants("Fract", "math/fract", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(0.4f, 0.9f, 0.39f, 0.67f));
        }

        [Test]
        public void TestNeg()
        {
            TestNodeWithAllFloatNInputVariants("Neg", "math/neg", new float4(15.4f, -10.1f, 12.39f, -32.33f), new float4(-15.4f, 10.1f, -12.39f, 32.33f));
        }

        [Test]
        public void TestRad()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.radians(a);

            TestNodeWithAllFloatNInputVariants("Rad", "math/rad", a, expected);
        }

        [Test]
        public void TestDeg()
        {
            var a = new float4(34.0f, 41.0f, 30.0f, 70.0f);
            var expected = math.degrees(a);

            TestNodeWithAllFloatNInputVariants("Deg", "math/deg", a, expected);
        }
    }
}