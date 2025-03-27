using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class MathNodesTests : InteractivityTestsHelpers
{
    private void TestOperationResult<T, TRes>(string nodeStr, T[] values, TRes expectedResult)
    {
        Graph g = new Graph();
        g.AddDefaultTypes();

        var onStartNode = g.CreateNode("event/onStart", Vector2.zero);
        var assertNode = g.CreateNode("debug/assert", Vector2.zero);
        onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);

        var opNode = g.CreateNode(nodeStr, Vector2.zero);

        Debug.Assert(values.Length <= ConstStrings.Letters.Length);
        for(int i = 0; i < values.Length; i++)
        {
            opNode.AddValue(ConstStrings.Letters[i], values[i]);
        }

        assertNode.AddValue(ConstStrings.B, expectedResult);

        if(assertNode.TryGetValueById(ConstStrings.A, out Value a))
        {
             a.TryConnectToSocket(opNode, ConstStrings.VALUE);
        }

        RunTestForGraph(g, null);
    }

    private void TestOperationResult<T>(string nodeStr, T val1, T expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1}, expectedResult);
    }

    private void TestOperationResult<T>(string nodeStr, T val1, T val2, T expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2}, expectedResult);
    }

    private void TestOperationResultT<T, TRes>(string nodeStr, T val1, TRes expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1}, expectedResult);
    }

    private void TestOperationResultT<T, TRes>(string nodeStr, T val1, T val2, TRes expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2}, expectedResult);
    }

    private void TestOperationResult<T>(string nodeStr, T val1, T val2, T val3, T expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2, val3}, expectedResult);
    }

    float4 tv1 = new float4(34.0f, 41.0f, 30.0f, 70.0f);
    float4 tv2 = new float4(30.0f, 40.0f, 50.0f, 60.0f);

    float4 tv3 = new float4(92.0f, 43.0f, 59.0f, 90.0f);

    private int tv1i = 10;
    private int tv2i = 20;

    private bool tv1b = true;
    private bool tv2b = false;


    private void TestMathOpAllFloats1op(string opStr, float4 expectedResult)
    {
        TestOperationResult(opStr, tv1, expectedResult);
        TestOperationResult(opStr, tv1.xyz, expectedResult.xyz);
        TestOperationResult(opStr, tv1.xy, expectedResult.xy);
        TestOperationResult(opStr, tv1.x, expectedResult.x);
    }

    private void TestMathOpAllFloats2op(string opStr, float4 expectedResult)
    {
        TestOperationResult(opStr, tv1, tv2, expectedResult);
        TestOperationResult(opStr, tv1.xyz, tv2.xyz, expectedResult.xyz);
        TestOperationResult(opStr, tv1.xy, tv2.xy, expectedResult.xy);
        TestOperationResult(opStr, tv1.x, tv2.x, expectedResult.x);
    }

    private void TestMathOpAllFloats3op(string opStr, float4 expectedResult)
    {
        TestOperationResult(opStr, tv1, tv2, tv3, expectedResult);
        TestOperationResult(opStr, tv1.xyz, tv2.xyz, tv3.xyz, expectedResult.xyz);
        TestOperationResult(opStr, tv1.xy, tv2.xy, tv3.xy, expectedResult.xy);
        TestOperationResult(opStr, tv1.x, tv2.x, tv3.x, expectedResult.x);
    }

    private void TestMathOpInt1op(string opStr, int expectedResult)
    {
        TestOperationResult(opStr, tv1i, expectedResult);
    }

    private void TestMathOpInt2op(string opStr, int expectedResult)
    {
        TestOperationResult(opStr, tv1i, tv2i, expectedResult);
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
    public void TestFloatToInt()
    {
        TestOperationResultT("type/floatToInt", tv1.x, (int)tv1.x);
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
