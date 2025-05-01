using NUnit.Framework;

namespace UnityGLTF.Interactivity.Tests
{
    public class TypeNodesTests : NodeTestHelpers
    {
        protected override string _subDirectory => "Type";

        [Test]
        public void IntToFloat()
        {
            QueueTest("IntToFloat_PositiveValue", CreateSelfContainedTestGraph("type/intToFloat", In(10), Out(10f), ComparisonType.Equals));
            QueueTest("IntToFloat_NegativeValue", CreateSelfContainedTestGraph("type/intToFloat", In(-10), Out(-10f), ComparisonType.Equals));
        }

        [Test]
        public void TestFloatToInt()
        {
            QueueTest("FloatToInt_Negative", CreateSelfContainedTestGraph("type/floatToInt", In(-10f), Out(-10), ComparisonType.Equals));
            QueueTest("FloatToInt_Positive", CreateSelfContainedTestGraph("type/floatToInt", In(10f), Out(10), ComparisonType.Equals));
            QueueTest("FloatToInt_Truncate_10.32", CreateSelfContainedTestGraph("type/floatToInt", In(10.32f), Out(10), ComparisonType.Equals));
            QueueTest("FloatToInt_Truncate_10.81", CreateSelfContainedTestGraph("type/floatToInt", In(10.81f), Out(10), ComparisonType.Equals));
            QueueTest("FloatToInt_Truncate_Neg_10.81", CreateSelfContainedTestGraph("type/floatToInt", In(-10.81f), Out(-10), ComparisonType.Equals));
            QueueTest("FloatToInt_Truncate_Neg_10.32", CreateSelfContainedTestGraph("type/floatToInt", In(-10.32f), Out(-10), ComparisonType.Equals));
        }

        [Test]
        public void TestBoolToInt()
        {
            QueueTest("BoolToInt_True", CreateSelfContainedTestGraph("type/boolToInt", In(true), Out(1), ComparisonType.Equals));
            QueueTest("BoolToInt_False", CreateSelfContainedTestGraph("type/boolToInt", In(false), Out(0), ComparisonType.Equals));
        }

        [Test]
        public void TestBoolToFloat()
        {
            QueueTest("BoolToFloat_True", CreateSelfContainedTestGraph("type/boolToFloat", In(true), Out(1f), ComparisonType.Equals));
            QueueTest("BoolToFloat_False", CreateSelfContainedTestGraph("type/boolToFloat", In(false), Out(0f), ComparisonType.Equals));
        }

        [Test]
        public void TestIntToBool()
        {
            QueueTest("IntToBool_One", CreateSelfContainedTestGraph("type/intToBool", In(1), Out(true), ComparisonType.Equals));
            QueueTest("IntToBool_Zero", CreateSelfContainedTestGraph("type/intToBool", In(0), Out(false), ComparisonType.Equals));
            QueueTest("IntToBool_Negative", CreateSelfContainedTestGraph("type/intToBool", In(-155), Out(true), ComparisonType.Equals));
        }

        [Test]
        public void TestFloatToBool()
        {
            QueueTest("FloatToBool_One", CreateSelfContainedTestGraph("type/floatToBool", In(1f), Out(true), ComparisonType.Equals));
            QueueTest("FloatToBool_Zero", CreateSelfContainedTestGraph("type/floatToBool", In(0f), Out(false), ComparisonType.Equals));
            QueueTest("FloatToBool_Negative", CreateSelfContainedTestGraph("type/floatToBool", In(-155f), Out(true), ComparisonType.Equals));
            QueueTest("FloatToBool_PositiveInfinity", CreateSelfContainedTestGraph("type/floatToBool", In(float.PositiveInfinity), Out(true), ComparisonType.Equals));
            QueueTest("FloatToBool_NaN", CreateSelfContainedTestGraph("type/floatToBool", In(float.NaN), Out(true), ComparisonType.Equals));
        }

    }
}