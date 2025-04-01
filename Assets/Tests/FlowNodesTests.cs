using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityGLTF.Interactivity;

public class FlowNodesTests : InteractivityTestsHelpers
{
    private void CreateFlowGraph<T>(string nodeStr, T condition, T endState)
    {
        //TODO generalize flow graph generation
    }

    private void CreateFlowBranchGraph<T>(string nodeStr, T condition, T expectedResult)
    {
        var graph = new Graph();
        graph.AddDefaultTypes();

        var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
        var branchNode = graph.CreateNode("flow/branch", Vector2.zero);
        var assertNode = graph.CreateNode("debug/assert", Vector2.zero);

        onStartNode.AddFlow(ConstStrings.OUT, branchNode, ConstStrings.IN);
        branchNode.AddFlow(ConstStrings.TRUE, assertNode, ConstStrings.IN);
        branchNode.AddFlow(ConstStrings.FALSE, assertNode, ConstStrings.IN);

        branchNode.AddValue(ConstStrings.CONDITION, condition);

        assertNode.AddValue(ConstStrings.A, expectedResult);
        assertNode.AddValue(ConstStrings.B, condition);

        RunTestForGraph(graph, null);
    }

    private void CreateFlowForGraph(int startIndex, int endIndex, int index = 0)
    {
        Graph g = new Graph();
        g.AddDefaultTypes();

        var onStartnode = g.CreateNode("event/onStart", Vector2.zero);
        var forNode = g.CreateNode("flow/for", Vector2.zero);
        var loopLogNode = g.CreateNode("debug/log", Vector2.zero);

        onStartnode.AddFlow(ConstStrings.OUT, forNode, ConstStrings.IN);
        forNode.AddFlow(ConstStrings.LOOP_BODY, loopLogNode, ConstStrings.IN);

        forNode.AddConfiguration(ConstStrings.INITIAL_INDEX, new JArray(index));
        forNode.AddValue(ConstStrings.START_INDEX, startIndex);
        forNode.AddValue(ConstStrings.END_INDEX, endIndex);

        if (loopLogNode.TryGetValueById("message", out Value a))
            a.TryConnectToSocket(forNode, ConstStrings.INDEX);

        RunTestForGraph(g, null);
    }

    private void TestFlowResult<T>(string nodeStr, T condition, T endVariable)
    {
        CreateFlowBranchGraph(nodeStr, condition, endVariable);
    }

    [Test]
    public void TestBranch()
    {
        TestFlowResult("flow/branch", true, true);
        TestFlowResult("flow/branch", false, false);        
    }

    [Test]
    public void TestFor()
    {
        CreateFlowForGraph(0, 6, 3);
        CreateFlowForGraph(0, 4);
    }

    [Test]
    public void TestSequence()
    {

    }

    [Test]
    public void TestSetDelay()
    {

    }

    [Test]
    public void TestCancelDelay()
    {

    }

    [Test]
    public void TestSwitch()
    {

    }
}
