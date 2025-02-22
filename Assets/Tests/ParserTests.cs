using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGLTF.Interactivity;

public class ParserTests
{
    [Test]
    public void ParserTestInt()
    {
        var expected = 5;
        var str = $"[\n                                        {expected}\n                                    ]";

        var parsed = Parser.ToInt(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestFloat()
    {
        var expected = 5.345f;
        var str = $"[\n                                        {expected}\n                                    ]";

        var parsed = Parser.ToFloat(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestVector2()
    {
        var expected = new Vector2(1f, 2f);

        var str = $"[\n                                        1.0,\n                                        2.0\n                                    ]";

        var parsed = Parser.ToVector2(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestVector3()
    {
        var expected = new Vector3(1f, 2f, 3f);

        var str = $"[\n                                        1.0,\n                                        2.0,\n                                        3.0\n                                    ]";

        var parsed = Parser.ToVector3(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestVector4()
    {
        var expected = new Vector4(1f, 2f, 3f, 4f);

        var str = $"[\n                                        1.0,\n                                        2.0,\n                                        3.0,\n                                        4.0\n                                    ]";

        var parsed = Parser.ToVector4(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestIntArray()
    {
        var expected = new int[] { 1, 3, 2, 5, 4 };

        var str = $"[\n                                        1,\n  3,\n  2,\n 5,\n 4\n                                   ]";

        var parsed = Parser.ToIntArray(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestBool()
    {
        var expected = true;
        var str = $"[\n                                        true\n                                    ]";

        var parsed = Parser.ToBool(str);

        Assert.AreEqual(expected, parsed);
    }

    [Test]
    public void ParserTestString()
    {
        var expected = "/nodes/{nodeIndex}/extensions/KHR_node_selectability/selectable";
        var str = $"[\n                                        \"{expected}\"\n                                    ]";

        var parsed = Parser.ToString(str);

        Assert.AreEqual(expected, parsed);
    }
}
