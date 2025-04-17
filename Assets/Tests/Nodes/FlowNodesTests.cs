using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TestTools;
using UnityGLTF.Interactivity;

namespace UnityGLTF.Interactivity.Tests
{
    public class FlowNodesTests : NodeTestHelpers
    {
        private void CreateFlowBranchGraph(bool outputSocketFromBranchNode)
        {
            var g = CreateGraphForTest();
            g.AddEvent("Assertion");

            var onStartNode = g.CreateNode("event/onStart");
            var branchNode = g.CreateNode("flow/branch");
            var failNode = g.CreateNode("event/send");
            var successNode = g.CreateNode("event/send");

            failNode.AddConfiguration(ConstStrings.EVENT, 0);
            successNode.AddConfiguration(ConstStrings.EVENT, 0);

            failNode.AddValue(ConstStrings.VALUE, false);
            successNode.AddValue(ConstStrings.VALUE, true);

            onStartNode.AddFlow(ConstStrings.OUT, branchNode, ConstStrings.IN);

            branchNode.AddFlow(ConstStrings.TRUE, outputSocketFromBranchNode ? successNode : failNode, ConstStrings.IN);
            branchNode.AddFlow(ConstStrings.FALSE, outputSocketFromBranchNode ? failNode : successNode, ConstStrings.IN);

            branchNode.AddValue(ConstStrings.CONDITION, outputSocketFromBranchNode);

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                if (eventIndex != 0)
                    return;

                var success = (Property<bool>)outValues[ConstStrings.VALUE];

                Assert.IsTrue(success.value);
            }
        }

        private void CreateFlowForGraph(int startIndex, int endIndex, int index = 0)
        {
            Graph g = CreateGraphForTest();

            var onStartnode = g.CreateNode("event/onStart");
            var forNode = g.CreateNode("flow/for");
            var loopBodySend = g.CreateNode("event/send");
            var completedSend = g.CreateNode("event/send");

            onStartnode.AddFlow(ConstStrings.OUT, forNode, ConstStrings.IN);
            forNode.AddFlow(ConstStrings.LOOP_BODY, loopBodySend, ConstStrings.IN);
            forNode.AddFlow(ConstStrings.COMPLETED, completedSend, ConstStrings.IN);

            forNode.AddConfiguration(ConstStrings.INITIAL_INDEX, new JArray(index));
            forNode.AddValue(ConstStrings.START_INDEX, startIndex);
            forNode.AddValue(ConstStrings.END_INDEX, endIndex);

            var loopBodyIndexValue = loopBodySend.AddValue(ConstStrings.VALUE, 0);
            loopBodyIndexValue.TryConnectToSocket(forNode, ConstStrings.INDEX);

            var completedIndexValue = loopBodySend.AddValue(ConstStrings.VALUE, 0);
            loopBodyIndexValue.TryConnectToSocket(forNode, ConstStrings.INDEX);

            g.AddEvent("Assertion");
            g.AddEvent("CompletedAssertion");

            loopBodySend.AddConfiguration(ConstStrings.EVENT, 0);
            completedSend.AddConfiguration(ConstStrings.EVENT, 1);

            var counter = index;

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                if (eventIndex == 0)
                {
                    var index = (Property<int>)outValues[ConstStrings.VALUE];

                    Assert.AreEqual(index.value, counter);
                    counter++;
                }
                else
                {
                    Assert.AreEqual(endIndex, counter);
                }
            }
        }

        private void CreateFlowSequenceGraph(int numOutFlows)
        {
            Graph g = CreateGraphForTest();

            var onStartnode = g.CreateNode("event/onStart");
            var sequenceNode = g.CreateNode("flow/sequence");

            var sendNodes = new Node[numOutFlows];

            onStartnode.AddFlow(ConstStrings.OUT, sequenceNode, ConstStrings.IN);

            for (int i = 0; i < sendNodes.Length; i++)
            {
                sendNodes[i] = g.CreateNode("event/send");
                sendNodes[i].AddValue(ConstStrings.VALUE, i);
                sendNodes[i].AddConfiguration(ConstStrings.EVENT, 0);
                sequenceNode.AddFlow(ConstStrings.Numbers[i], sendNodes[i], ConstStrings.IN);
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
            g.AddEvent("cancel"); // 3

            var onStartnode = g.CreateNode("event/onStart"); // 0
            var setDelayNode = g.CreateNode("flow/setDelay"); // 1
            var outSendNode = g.CreateNode("event/send"); // 2
            var errSendNode = g.CreateNode("event/send"); // 3
            var doneSendNode = g.CreateNode("event/send"); // 4
            var cancelReceiveNode = g.CreateNode("event/receive"); // 5

            outSendNode.AddConfiguration(ConstStrings.EVENT, 0);
            errSendNode.AddConfiguration(ConstStrings.EVENT, 1);
            doneSendNode.AddConfiguration(ConstStrings.EVENT, 2);
            cancelReceiveNode.AddConfiguration(ConstStrings.EVENT, 3);

            onStartnode.AddFlow(ConstStrings.OUT, setDelayNode, ConstStrings.IN);
            cancelReceiveNode.AddFlow(ConstStrings.OUT, setDelayNode, ConstStrings.CANCEL);

            setDelayNode.AddFlow(ConstStrings.OUT, outSendNode, ConstStrings.IN);
            setDelayNode.AddFlow(ConstStrings.ERR, errSendNode, ConstStrings.IN);
            setDelayNode.AddFlow(ConstStrings.DONE, doneSendNode, ConstStrings.IN);

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

        [Test]
        public void TestCancelDelay()
        {

        }

        [Test]
        public void TestSwitch()
        {

        }
    }
}