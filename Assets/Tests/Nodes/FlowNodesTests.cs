using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TestTools;
using UnityGLTF.Interactivity;

namespace UnityGLTF.Interactivity.Tests
{
    public class FlowNodesTests : NodeTestHelpers
    {
        void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
        {
            switch(eventIndex)
            {
                case 0:
                    Assert.Fail("Failure case was triggered by this test.");
                    break;
                case 1:
                    _testSuccessful = true;
                    break;
            }
        }

        private void CreateFlowBranchGraph(bool outputSocketFromBranchNode)
        {
            var g = CreateGraphForTest();

            var onStartNode = g.CreateNode("event/onStart");
            var branchNode = g.CreateNode("flow/branch");
            var failNode = g.CreateNode("event/send");
            var successNode = g.CreateNode("event/send");

            failNode.AddConfiguration(ConstStrings.EVENT, 0);
            successNode.AddConfiguration(ConstStrings.EVENT, 1);

            onStartNode.AddFlow(ConstStrings.OUT, branchNode);

            branchNode.AddFlow(ConstStrings.TRUE, outputSocketFromBranchNode ? successNode : failNode);
            branchNode.AddFlow(ConstStrings.FALSE, outputSocketFromBranchNode ? failNode : successNode);

            branchNode.AddValue(ConstStrings.CONDITION, outputSocketFromBranchNode);

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }

        private Graph CreateDoNResetGraph(int n, int resetAfter)
        {
            const int COUNTER_VARIABLE_INITIAL_VALUE = 0;
            const string TOTAL_COUNT = "totalCount";
            var expectedTotalCount = 2 * resetAfter;
            Graph g = CreateGraphForTest();

            var resetVariable = g.AddVariable("hasResetOnce", false);

            var start = g.CreateNode("event/onStart");
            var startTrigger = g.CreateNode("event/send");
            var triggerReceive = g.CreateNode("event/receive");
            var resetReceive = g.CreateNode("event/receive");
            var doNNode = g.CreateNode("flow/doN");
            var triggerSend = g.CreateNode("event/send");
            var resetSend = g.CreateNode("event/send");
            var failSend = g.CreateNode("event/send");
            var failTotalSend = g.CreateNode("event/send");
            var completeSend = g.CreateNode("event/send");
            var eq = g.CreateNode("math/eq");
            var currentCountGet = g.CreateNode("variable/get");
            var loopBodyBranch = g.CreateNode("flow/branch");
            var resetBranch = g.CreateNode("flow/branch");
            var resetSet = g.CreateNode("variable/set");
            var loopEq = g.CreateNode("math/eq");
            var resetGet = g.CreateNode("variable/get");
            var resetOrFinishBranch = g.CreateNode("flow/branch");
            var resetTrigger = g.CreateNode("event/send");
            var resetCounterVariable = g.CreateNode("variable/set");
            var totalCountGet = g.CreateNode("variable/get");
            var totalCountBranch = g.CreateNode("flow/branch");
            var totalCountEq = g.CreateNode("math/eq");
            var completedFailLog = g.CreateNode("debug/log");

            completedFailLog.AddConfiguration(ConstStrings.MESSAGE, "Completed with Total Iterations Expected: {expected}, Actual: {actual}");
            completedFailLog.AddValue(ConstStrings.EXPECTED, expectedTotalCount);

            completedFailLog.AddConnectedValue(ConstStrings.ACTUAL, totalCountGet);

            resetSet.AddConfiguration(ConstStrings.VARIABLE, g.IndexOfVariable(resetVariable));
            resetGet.AddConfiguration(ConstStrings.VARIABLE, g.IndexOfVariable(resetVariable));

            Node varSet = CreateVariableIncrementSubgraph(g, ConstStrings.CURRENT_COUNT, COUNTER_VARIABLE_INITIAL_VALUE);
            Node totalCountSet = CreateVariableIncrementSubgraph(g, TOTAL_COUNT, COUNTER_VARIABLE_INITIAL_VALUE);

            currentCountGet.AddConfiguration(ConstStrings.VARIABLE, g.IndexOfVariable(ConstStrings.CURRENT_COUNT));
            resetCounterVariable.AddConfiguration(ConstStrings.VARIABLE, g.IndexOfVariable(ConstStrings.CURRENT_COUNT));
            totalCountGet.AddConfiguration(ConstStrings.VARIABLE, g.IndexOfVariable(TOTAL_COUNT));

            triggerReceive.AddFlow(ConstStrings.OUT, doNNode);
            resetReceive.AddFlow(ConstStrings.OUT, doNNode, ConstStrings.RESET);
            doNNode.AddFlow(ConstStrings.OUT, totalCountSet);
            totalCountSet.AddFlow(ConstStrings.OUT, varSet);

            varSet.AddFlow(ConstStrings.OUT, loopBodyBranch);

            doNNode.AddValue(ConstStrings.N, n);
            eq.AddConnectedValue(ConstStrings.A, doNNode, ConstStrings.CURRENT_COUNT);
            eq.AddConnectedValue(ConstStrings.B, currentCountGet);

            loopBodyBranch.AddConnectedValue(ConstStrings.CONDITION, eq);

            loopBodyBranch.AddFlow(ConstStrings.TRUE, resetBranch);
            loopBodyBranch.AddFlow(ConstStrings.FALSE, failSend);
            resetBranch.AddFlow(ConstStrings.TRUE, resetOrFinishBranch);
            resetBranch.AddFlow(ConstStrings.FALSE, triggerSend);

            resetBranch.AddConnectedValue(ConstStrings.CONDITION, loopEq);

            loopEq.AddConnectedValue(ConstStrings.A, doNNode, ConstStrings.CURRENT_COUNT);
            loopEq.AddValue(ConstStrings.B, resetAfter);

            resetOrFinishBranch.AddConnectedValue(ConstStrings.CONDITION, resetGet);

            resetOrFinishBranch.AddFlow(ConstStrings.TRUE, totalCountBranch);
            resetOrFinishBranch.AddFlow(ConstStrings.FALSE, resetSet);

            totalCountBranch.AddConnectedValue(ConstStrings.CONDITION, totalCountEq);
            totalCountBranch.AddFlow(ConstStrings.TRUE, completeSend);
            totalCountBranch.AddFlow(ConstStrings.FALSE, completedFailLog);
            completedFailLog.AddFlow(ConstStrings.OUT, failTotalSend);
            resetSet.AddFlow(ConstStrings.OUT, resetCounterVariable);
            resetSet.AddValue(ConstStrings.VALUE, true);

            resetCounterVariable.AddValue(ConstStrings.VALUE, COUNTER_VARIABLE_INITIAL_VALUE);
            resetCounterVariable.AddFlow(ConstStrings.OUT, resetSend);
            resetSend.AddFlow(ConstStrings.OUT, resetTrigger);

            totalCountEq.AddConnectedValue(ConstStrings.A, totalCountGet);
            totalCountEq.AddValue(ConstStrings.B, expectedTotalCount);

            g.AddEvent("TriggerDoN");
            g.AddEvent("Reset");

            start.AddFlow(ConstStrings.OUT, startTrigger);

            startTrigger.AddConfiguration(ConstStrings.EVENT, 2);
            failSend.AddConfiguration(ConstStrings.EVENT, 0);
            failTotalSend.AddConfiguration(ConstStrings.EVENT, 0);
            completeSend.AddConfiguration(ConstStrings.EVENT, 1);
            triggerSend.AddConfiguration(ConstStrings.EVENT, 2);
            triggerReceive.AddConfiguration(ConstStrings.EVENT, 2);
            resetTrigger.AddConfiguration(ConstStrings.EVENT, 2);
            resetReceive.AddConfiguration(ConstStrings.EVENT, 3);
            resetSend.AddConfiguration(ConstStrings.EVENT, 3);

            GraphSerializer serializer = new(Newtonsoft.Json.Formatting.Indented);
            var extension = new KHR_interactivity()
            {
                graphs = new List<Graph>() { g },
                defaultGraphIndex = 0
            };
            File.WriteAllText($"{Application.streamingAssetsPath}/outputTestGraph.json", serializer.Serialize(extension));

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            return g;
        }

        private void TestFlowForGraph(int startIndex, int endIndex, int initialIndex)
        {
            Graph g = CreateGraphForTest();

            var onStartnode = g.CreateNode("event/onStart");
            var forNode = g.CreateNode("flow/for");
            var loopBodyBranch = g.CreateNode("flow/branch");
            var completedBranch = g.CreateNode("flow/branch");
            var loopBodySend = g.CreateNode("event/send");
            var completedSend = g.CreateNode("event/send");
            var successSend = g.CreateNode("event/send");
            var loopBodyGet = g.CreateNode("variable/get");
            var loopBodyEq = g.CreateNode("math/eq");
            var completedEq = g.CreateNode("math/eq");
            var loopFailLog = g.CreateNode("debug/log");
            var completedFailLog = g.CreateNode("debug/log");
            Node varSet = CreateVariableIncrementSubgraph(g, ConstStrings.INDEX, startIndex - 1);

            loopFailLog.AddConfiguration(ConstStrings.MESSAGE, "Loop Body with Index Expected: {expected}, Actual: {actual}");
            completedFailLog.AddConfiguration(ConstStrings.MESSAGE, "Completed with Index Expected: {expected}, Actual: {actual}");

            loopBodyGet.AddConfiguration(ConstStrings.VARIABLE, 0);

            loopBodySend.AddConfiguration(ConstStrings.EVENT, 0);
            completedSend.AddConfiguration(ConstStrings.EVENT, 0);
            successSend.AddConfiguration(ConstStrings.EVENT, 1);

            onStartnode.AddFlow(ConstStrings.OUT, forNode);
            forNode.AddFlow(ConstStrings.LOOP_BODY, varSet);
            forNode.AddFlow(ConstStrings.COMPLETED, completedBranch);

            forNode.AddConfiguration(ConstStrings.INITIAL_INDEX, initialIndex);
            forNode.AddValue(ConstStrings.START_INDEX, startIndex);
            forNode.AddValue(ConstStrings.END_INDEX, endIndex);

            varSet.AddFlow(ConstStrings.OUT, loopBodyBranch);
            loopBodyBranch.AddConnectedValue(ConstStrings.CONDITION, loopBodyEq);

            loopBodyEq.AddConnectedValue(ConstStrings.A, forNode, ConstStrings.INDEX);
            loopBodyEq.AddConnectedValue(ConstStrings.B, loopBodyGet);

            loopBodyBranch.AddFlow(ConstStrings.FALSE, loopFailLog);
            completedBranch.AddFlow(ConstStrings.FALSE, completedFailLog);
            completedBranch.AddFlow(ConstStrings.TRUE, successSend);

            loopFailLog.AddFlow(ConstStrings.OUT, loopBodySend);
            completedFailLog.AddFlow(ConstStrings.OUT, completedSend);

            loopFailLog.AddConnectedValue(ConstStrings.EXPECTED, loopBodyGet);
            loopFailLog.AddConnectedValue(ConstStrings.ACTUAL, forNode, ConstStrings.INDEX);

            completedFailLog.AddValue(ConstStrings.EXPECTED, endIndex);
            completedFailLog.AddConnectedValue(ConstStrings.ACTUAL, forNode, ConstStrings.INDEX);

            completedBranch.AddConnectedValue(ConstStrings.CONDITION, completedEq);

            completedEq.AddValue(ConstStrings.A, endIndex);
            completedEq.AddConnectedValue(ConstStrings.B, forNode, ConstStrings.INDEX);

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }

        private static Node CreateVariableIncrementSubgraph(Graph g, string variableName, int initialValue)
        {
            var variable = g.AddVariable(variableName, initialValue);
            var variableIndex = g.IndexOfVariable(variable);

            var varSet = g.CreateNode("variable/set");
            var varSetGet = g.CreateNode("variable/get");
            var add = g.CreateNode("math/add");

            varSet.AddConfiguration(ConstStrings.VARIABLE, variableIndex);
            varSetGet.AddConfiguration(ConstStrings.VARIABLE, variableIndex);

            varSet.AddConnectedValue(ConstStrings.VALUE, add);

            add.AddValue(ConstStrings.B, 1);
            add.AddConnectedValue(ConstStrings.A, varSetGet);
            return varSet;
        }

        private void CreateFlowSequenceGraph(int numOutFlows)
        {
            Graph g = CreateGraphForTest();

            var onStartnode = g.CreateNode("event/onStart");
            var sequenceNode = g.CreateNode("flow/sequence");

            var sendNodes = new Node[numOutFlows];

            onStartnode.AddFlow(ConstStrings.OUT, sequenceNode);

            for (int i = 0; i < sendNodes.Length; i++)
            {
                sendNodes[i] = g.CreateNode("event/send");
                sendNodes[i].AddValue(ConstStrings.VALUE, i);
                sendNodes[i].AddConfiguration(ConstStrings.EVENT, 0);
                sequenceNode.AddFlow(ConstStrings.Numbers[i], sendNodes[i]);
            }

            g.AddEvent("Result");

            var counter = 0;

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                var index = (Property<int>)outValues[ConstStrings.VALUE];

                Assert.AreEqual(index.value, counter);

                counter++;
            }
        }

        private Graph CreateSetDelayGraph(float duration)
        {
            Graph g = CreateGraphForTest();
            g.AddEvent("out"); // 0
            g.AddEvent("err"); // 1
            g.AddEvent("done"); // 2

            var onStartnode = g.CreateNode("event/onStart"); // 0
            var setDelayNode = g.CreateNode("flow/setDelay"); // 1
            var outSendNode = g.CreateNode("event/send"); // 2
            var errSendNode = g.CreateNode("event/send"); // 3
            var doneSendNode = g.CreateNode("event/send"); // 4

            outSendNode.AddConfiguration(ConstStrings.EVENT, 0);
            errSendNode.AddConfiguration(ConstStrings.EVENT, 1);
            doneSendNode.AddConfiguration(ConstStrings.EVENT, 2);

            onStartnode.AddFlow(ConstStrings.OUT, setDelayNode);

            setDelayNode.AddFlow(ConstStrings.OUT, outSendNode);
            setDelayNode.AddFlow(ConstStrings.ERR, errSendNode);
            setDelayNode.AddFlow(ConstStrings.DONE, doneSendNode);

            setDelayNode.AddValue(ConstStrings.DURATION, duration);

            return g;
        }

        [Test]
        public void TestBranch()
        {
            CreateFlowBranchGraph(false);
            CreateFlowBranchGraph(true);
        }

        [Test]
        public void TestDoN()
        {
            CreateDoNGraph(5);
        }

        [Test]
        public void TestDoNWithReset()
        {
            CreateDoNResetGraph(5, 3);
        }

        [Test]
        public void TestFor()
        {
            TestFlowForGraph(0, 6, 3);
            TestFlowForGraph(0, 4, 0);
        }

        [Test]
        public void TestSequence()
        {
            CreateFlowSequenceGraph(5);
        }

        [UnityTest]
        public IEnumerator TestSetDelay()
        {
            const float DURATION = 0.75f;
            const float MAX_ERROR = 0.01f;
            const float EXTRA_EXECUTION_TIME = 0.25f;
            var g = CreateSetDelayGraph(DURATION);

            var startTime = Time.time;
            var endTime = Time.time + DURATION;
            var finishExecutionTime = endTime + EXTRA_EXECUTION_TIME;

            var outFlowExecuted = false;
            var doneFlowExecuted = false;

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            while (Time.time < finishExecutionTime)
            {
                eng.Tick();
                yield return null;
            }

            Assert.IsTrue(outFlowExecuted);
            Assert.IsTrue(doneFlowExecuted);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                switch(eventIndex)
                {
                    case 0:
                        Debug.Log($"Out triggered at {Time.time}, should be very close to {startTime}");
                        outFlowExecuted = true;
                        UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.time, startTime, MAX_ERROR);
                        return;

                    case 1:
                        Assert.Fail("Should never get the err flow triggered in this test.");
                        return;

                    case 2:
                        Debug.Log($"Done triggered at {Time.time}, should be very close to {endTime}");
                        doneFlowExecuted = true;
                        UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.time, endTime, MAX_ERROR);
                        return;
                }
            }
        }


        [Test]
        public void TestSetDelayInvalidDurationNaN()
        {
            TestSetDelayInvalidDuration(float.NaN);
        }

        [Test]
        public void TestSetDelayInvalidDurationNegative()
        {
            TestSetDelayInvalidDuration(-1f);
        }

        [Test]
        public void TestSetDelayInvalidDurationInfinite()
        {
            TestSetDelayInvalidDuration(float.PositiveInfinity);
        }

        private void TestSetDelayInvalidDuration(float duration)
        {
            var g = CreateSetDelayGraph(duration);

            var startTime = Time.time;

            var errFlowExecuted = false;

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(errFlowExecuted);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                switch (eventIndex)
                {
                    case 0:
                        Assert.Fail("Should never get the out flow triggered in this test.");
                        return;

                    case 1:
                        errFlowExecuted = true;
                        Debug.Log($"Error flow triggered as duration is {duration} which is invalid.");
                        return;

                    case 2:
                        Assert.Fail("Should never get the done flow triggered in this test.");
                        return;
                }
            }
        }

        [UnityTest]
        public IEnumerator TestSetDelayCancelWithEventReceive()
        {
            const float DURATION = 0.75f;
            const float CANCEL_TIME = 0.35f;
            const float MAX_ERROR = 0.01f;
            const float EXTRA_EXECUTION_TIME = 0.25f;
            var g = CreateSetDelayGraph(DURATION);

            // Add event/receive node that triggers the "cancel" input flow of the setDelay node when the cancel event is triggered.
            g.AddEvent("cancel"); // 3
            var cancelReceiveNode = g.CreateNode("event/receive");
            cancelReceiveNode.AddConfiguration(ConstStrings.EVENT, 3);
            cancelReceiveNode.AddFlow(ConstStrings.OUT, g.nodes[1], ConstStrings.CANCEL);

            var startTime = Time.time;
            var endTime = Time.time + DURATION;
            var finishExecutionTime = endTime + EXTRA_EXECUTION_TIME;

            var outFlowExecuted = false;

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            var hasCancelled = false;

            while (Time.time < finishExecutionTime)
            {
                if (!hasCancelled && Time.time > CANCEL_TIME)
                {
                    eng.FireCustomEvent(3);
                    hasCancelled = true;
                }

                eng.Tick();
                yield return null;
            }

            var setDelayEngineNode = eng.engineNodes[g.nodes[1]];
            var lastDelayIndex = (Property<int>)setDelayEngineNode.GetOutputValue(ConstStrings.LAST_DELAY_INDEX);
            Assert.AreEqual(-1, lastDelayIndex.value);
            Assert.IsTrue(outFlowExecuted);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                switch (eventIndex)
                {
                    case 0:
                        Debug.Log($"Out triggered at {Time.time}, should be very close to {startTime}");
                        outFlowExecuted = true;
                        UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.time, startTime, MAX_ERROR);
                        return;

                    case 1:
                        Assert.Fail("Should never get the err flow triggered in this test.");
                        return;

                    case 2:
                        Assert.Fail("Should never get the done flow triggered in this test.");
                        return;
                }
            }
        }

        [UnityTest]
        public IEnumerator TestCancelDelay()
        {
            const float DURATION = 0.75f;
            const float CANCEL_TIME = 0.35f;
            const float MAX_ERROR = 0.01f;
            const float EXTRA_EXECUTION_TIME = 0.25f;
            var g = CreateSetDelayGraph(DURATION);

            // Add cancelDelay node
            var cancelDelayNode = g.CreateNode("flow/cancelDelay");
            cancelDelayNode.AddValue(ConstStrings.DELAY_INDEX, 0);

            // Add event/receive node that triggers the cancelDelay node when the cancel event is triggered.
            g.AddEvent("cancel"); // 3
            var cancelReceiveNode = g.CreateNode("event/receive");
            cancelReceiveNode.AddConfiguration(ConstStrings.EVENT, 3);
            cancelReceiveNode.AddFlow(ConstStrings.OUT, cancelDelayNode);

            var startTime = Time.time;
            var endTime = Time.time + DURATION;
            var finishExecutionTime = endTime + EXTRA_EXECUTION_TIME;

            var outFlowExecuted = false;

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            var hasCancelled = false;

            while (Time.time < finishExecutionTime)
            {
                if (!hasCancelled && Time.time > CANCEL_TIME)
                {
                    eng.FireCustomEvent(3);
                    hasCancelled = true;
                }
                eng.Tick();
                yield return null;
            }

            Assert.IsTrue(outFlowExecuted);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                switch (eventIndex)
                {
                    case 0:
                        Debug.Log($"Out triggered at {Time.time}, should be very close to {startTime}");
                        outFlowExecuted = true;
                        UnityEngine.Assertions.Assert.AreApproximatelyEqual(Time.time, startTime, MAX_ERROR);
                        return;

                    case 1:
                        Assert.Fail("Should never get the err flow triggered in this test.");
                        return;

                    case 2:
                        Assert.Fail("Should never get the done flow triggered in this test.");
                        return;
                }
            }
        }

        [Test]
        public void TestSwitch()
        {

        }
    }
}