using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;

public class MathNodesTests : InteractivityTestsHelpers
{
    private void TestOperationResult<T>(string nodeStr, T[] values, T expectedResult)
    {
        Graph g = new Graph();
        g.AddDefaultTypes();

        var onStartNode = g.CreateNode("event/onStart", Vector2.zero);
        var assertNode = g.CreateNode("debug/assert", Vector2.zero);
        onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);


        var opNode = g.CreateNode(nodeStr, Vector2.zero);

        string[] constStrings = new string[5] {ConstStrings.A, ConstStrings.B, ConstStrings.C, ConstStrings.D, ConstStrings.E};
        Debug.Assert(values.Length <= constStrings.Length);
        for(int i = 0; i < values.Length; i++)
        {
            opNode.AddValue(constStrings[i], values[i]);
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

    float4 tv1 = new float4(10.0f, 20.0f, 30.0f, 40.0f);
    float4 tv2 = new float4(30.0f, 40.0f, 50.0f, 60.0f);

    private int tv1i = 10;
    private int tv2i = 20;

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
    public void TestAbs()
    {
        TestMathOpAllFloats1op("math/abs", math.abs(tv1));
        TestMathOpInt1op("math/abs", (int)math.abs(tv1i));
    }
}
