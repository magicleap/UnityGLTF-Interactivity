using NUnit.Framework;

namespace UnityGLTF.Interactivity.Tests
{
    public class TypeNodesTests : NodeTestHelpers
    {
        [Test]
        public void TestIntToFloat()
        {
            TestNode("type/intToFloat", 10, 10f);
            TestNode("type/intToFloat", -10, -10f);
        }

        [Test]
        public void TestFloatToInt()
        {
            TestNode("type/floatToInt", -10f, -10);
            TestNode("type/floatToInt", 10f, 10);
            TestNode("type/floatToInt", 10.32f, 10);
            TestNode("type/floatToInt", 10.81f, 10);
            TestNode("type/floatToInt", -10.81f, -10);
            TestNode("type/floatToInt", -10.32f, -10);
        }

        [Test]
        public void TestBoolToInt()
        {
            TestNode("type/boolToInt", true, 1);
            TestNode("type/boolToInt", false, 0);
        }

        [Test]
        public void TestBoolToFloat()
        {
            TestNode("type/boolToFloat", true, 1f);
            TestNode("type/boolToFloat", false, 0f);
        }

        [Test]
        public void TestIntToBool()
        {
            TestNode("type/intToBool", 1, true);
            TestNode("type/intToBool", 0, false);
            TestNode("type/intToBool", -155, true);
        }

        [Test]
        public void TestFloatToBool()
        {
            TestNode("type/intToBool", 1f, true);
            TestNode("type/intToBool", 0f, false);
            TestNode("type/intToBool", -155f, true);
            TestNode("type/intToBool", float.PositiveInfinity, true);
            TestNode("type/intToBool", float.NaN, true);
        }
    }
}