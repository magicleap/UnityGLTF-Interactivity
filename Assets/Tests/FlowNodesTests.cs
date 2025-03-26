using System;
using NUnit.Framework;
using UnityEngine;
using UnityGLTF.Interactivity;

public class FlowNodesTests : InteractivityTestsHelpers
{
    private void CreateFlowBranchGraph(bool condition)
    {
        var graph = new Graph();
        graph.AddDefaultTypes();

        var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
        var flowBranchNode = graph.CreateNode("flow/branch", Vector2.zero);

        flowBranchNode.AddValue("condition", condition);

        onStartNode.AddFlow(ConstStrings.OUT, flowBranchNode, ConstStrings.IN);

        RunTestForGraph(graph, null);
    }

    private void TestBranchResult(bool expectedResult)
    {
        CreateFlowBranchGraph(expectedResult);
    }

    [Test]
    public void TestBranch()
    {
        TestBranchResult(true);
    }

    [Test]
    public void TestBranchFalse()
    {
        TestBranchResult(false);
    }
}
