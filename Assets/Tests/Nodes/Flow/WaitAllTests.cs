using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityGLTF.Interactivity.Tests
{
    partial class FlowNodesTests
    {
        [Test]
        public void TestWaitAll()
        {
            var g = GenerateWaitAllTestGraph(5);

            GraphSerializer serializer = new(Newtonsoft.Json.Formatting.Indented);
            var extension = new KHR_interactivity()
            {
                graphs = new List<Graph>() { g },
                defaultGraphIndex = 0
            };
            File.WriteAllText($"{Application.streamingAssetsPath}/outputTestGraph.json", serializer.Serialize(extension));

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }

        [Test]
        public void TestWaitAllReset()
        {
            var g = GenerateWaitAllResetTestGraph();

            GraphSerializer serializer = new(Newtonsoft.Json.Formatting.Indented);
            var extension = new KHR_interactivity()
            {
                graphs = new List<Graph>() { g },
                defaultGraphIndex = 0
            };
            File.WriteAllText($"{Application.streamingAssetsPath}/outputTestGraph.json", serializer.Serialize(extension));

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }

        private static Graph GenerateWaitAllTestGraph(int inputs)
        {
            Graph g = CreateGraphForTest();

            var starts = new Node[inputs];
            var waitAll = g.CreateNode("flow/waitAll");
            waitAll.AddConfiguration(ConstStrings.INPUT_FLOWS, inputs);

            for (int i = 0; i < inputs; i++)
            {
                starts[i] = g.CreateNode("event/onStart");
                starts[i].AddFlow(waitAll, ConstStrings.OUT, ConstStrings.Numbers[i]);
            }

            var completed = g.CreateNode("event/send");
            completed.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);

            waitAll.AddFlow(completed, ConstStrings.COMPLETED);

            return g;
        }

        private static Graph GenerateWaitAllResetTestGraph()
        {
            const int INPUTS = 5;
            Graph g = CreateGraphForTest();

            var start = g.CreateNode("event/onStart");
            var waitAll = g.CreateNode("flow/waitAll");
            var sequence = g.CreateNode("flow/sequence");
            waitAll.AddConfiguration(ConstStrings.INPUT_FLOWS, INPUTS);

            start.AddFlow(sequence);

            var branch1 = g.CreateNode("flow/branch");
            var eq1 = g.CreateNode("math/eq");

            branch1.AddConnectedValue(ConstStrings.CONDITION, eq1);

            eq1.AddConnectedValue(ConstStrings.A, waitAll, ConstStrings.REMAINING_INPUTS);
            eq1.AddValue(ConstStrings.B, 2);

            var inputs1Log = g.CreateNode("debug/log");
            var fail1 = g.CreateNode("event/send");

            fail1.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);

            inputs1Log.AddFlow(fail1);
            inputs1Log.AddConfiguration(ConstStrings.MESSAGE, "remainingInputs, Expected: 2, Actual: {actual}");
            inputs1Log.AddConnectedValue(ConstStrings.ACTUAL, waitAll, ConstStrings.REMAINING_INPUTS);

            branch1.AddFlow(inputs1Log, ConstStrings.FALSE);

            var branch2 = g.CreateNode("flow/branch");
            var eq2 = g.CreateNode("math/eq");

            branch2.AddConnectedValue(ConstStrings.CONDITION, eq2);

            eq2.AddConnectedValue(ConstStrings.A, waitAll, ConstStrings.REMAINING_INPUTS);
            eq2.AddValue(ConstStrings.B, 3);

            var inputs2Log = g.CreateNode("debug/log");
            var fail2 = g.CreateNode("event/send");

            fail2.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);

            inputs2Log.AddFlow(fail2);
            inputs2Log.AddConfiguration(ConstStrings.MESSAGE, "remainingInputs, Expected: 3, Actual: {actual}");
            inputs2Log.AddConnectedValue(ConstStrings.ACTUAL, waitAll, ConstStrings.REMAINING_INPUTS);

            branch2.AddFlow(inputs2Log, ConstStrings.FALSE);

            sequence.AddFlow(waitAll, ConstStrings.Numbers[0], ConstStrings.Numbers[0]);
            sequence.AddFlow(waitAll, ConstStrings.Numbers[1], ConstStrings.Numbers[1]);
            sequence.AddFlow(waitAll, ConstStrings.Numbers[2], ConstStrings.Numbers[2]);
            sequence.AddFlow(branch1, ConstStrings.Numbers[3]);
            sequence.AddFlow(waitAll, ConstStrings.Numbers[4], ConstStrings.RESET);
            sequence.AddFlow(waitAll, ConstStrings.Numbers[5], ConstStrings.Numbers[3]);
            sequence.AddFlow(waitAll, ConstStrings.Numbers[6], ConstStrings.Numbers[4]);
            sequence.AddFlow(branch2, ConstStrings.Numbers[7]);

            var completed = g.CreateNode("event/send");
            completed.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);

            branch2.AddFlow(completed, ConstStrings.TRUE);

            return g;
        }
    }
}