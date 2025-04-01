using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Pool;
using UnityGLTF.Interactivity;

public class FlowNodesTests : InteractivityTestsHelpers
{
    private void CreateFlowGraph<T>(string nodeStr, T condition, T endState)
    {
        //TODO generalize flow graph generation
    }

    private void CreateFlowBranchGraph<T>(T condition, T expectedResult)
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

    private void CreateFlowSequenceGraph(int numOutFlows)
    {
        Graph g = new Graph();
        g.AddDefaultTypes();

        var onStartnode = g.CreateNode("event/onStart", Vector2.zero);
        var sequenceNode = g.CreateNode("flow/sequence", Vector2.zero);

        onStartnode.AddFlow(ConstStrings.OUT, sequenceNode, ConstStrings.IN);

        var seqFlows = ConstStrings.Numbers[0..numOutFlows];
        var rand = Random.Range(0, numOutFlows);
        var rand2 = Random.Range(0, numOutFlows);
        Debug.Log($"Swapping values {seqFlows[rand]} and {seqFlows[rand2]} to verify order execution");
        var temp = seqFlows[rand];
        seqFlows[rand] = seqFlows[rand2];
        seqFlows[rand2] = temp;

        for (int i = 0; i < numOutFlows; i++)
        {
            var logNode = g.CreateNode("debug/log", Vector2.zero);
            logNode.AddValue("message", $"This is the next flow in the sequence: {seqFlows[i]}.");
            sequenceNode.AddFlow(seqFlows[i], logNode, ConstStrings.IN);
        }

        RunTestForGraph(g, null);
    }

    [Test]
    public void TestBranch()
    {
        CreateFlowBranchGraph(false, false);
        CreateFlowBranchGraph(false, false);   
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
        CreateFlowSequenceGraph(5);
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
