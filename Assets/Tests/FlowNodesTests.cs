using System;
using NUnit.Framework;
using UnityEngine;
using UnityGLTF.Interactivity;

public class FlowNodesTests : InteractivityTestsHelpers
{
    private int tv1 = 1;

    private void CreateFlowBranchGraph<T>(string nodeStr, bool condition, T value, T expectedResult)
    {
        var graph = new Graph();
        graph.AddDefaultTypes();
        
        var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
        var flowNode = graph.CreateNode(nodeStr, Vector2.zero);
        var assertTrueNode = graph.CreateNode("debug/assert", Vector2.zero);
        var assertFalseNode = graph.CreateNode("debug/assert", Vector2.zero);

        onStartNode.AddFlow(ConstStrings.OUT, flowNode, ConstStrings.IN);
        flowNode.AddFlow(ConstStrings.TRUE, assertTrueNode, ConstStrings.IN);
        flowNode.AddFlow(ConstStrings.FALSE, assertFalseNode, ConstStrings.IN);

        flowNode.AddValue(ConstStrings.CONDITION, condition);
        assertTrueNode.AddValue(ConstStrings.A, expectedResult);
        assertTrueNode.AddValue(ConstStrings.B, tv1);
        assertFalseNode.AddValue(ConstStrings.A, expectedResult);
        assertFalseNode.AddValue(ConstStrings.B, 0);
        
        RunTestForGraph(graph, null);
    }

    private void TestFlowResult<T>(string nodeStr, bool condition, T val1, T expectedOutcome)
    {
        CreateFlowBranchGraph(nodeStr, condition, val1, expectedOutcome);
    }

    [Test]
    public void TestBranch()
    {
        TestFlowResult("flow/branch", true, tv1, tv1);
        TestFlowResult("flow/branch", false, tv1, 0);
    }
}
