using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityGLTF.Interactivity.Tests
{
    public class EventNodeTests : NodeTestHelpers
    {
        void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
        {
            switch (eventIndex)
            {
                case 0:
                    Assert.Fail("Failure case was triggered by this test.");
                    break;
                case 1:
                    _testSuccessful = true;
                    break;
            }
        }

        [Test]
        public void TestOnStart()
        {
            var g = CreateGraphForTest();

            var start = g.CreateNode("event/onStart");
            var complete = g.CreateNode("event/send");

            start.AddFlow(complete);

            complete.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }

        [UnityTest]
        public IEnumerator TestOnTick()
        {
            Graph g = CreateOnTickTestGraph();

            GraphSerializer serializer = new(Newtonsoft.Json.Formatting.Indented);
            var extension = new KHR_interactivity()
            {
                graphs = new List<Graph>() { g },
                defaultGraphIndex = 0
            };
            File.WriteAllText($"{Application.streamingAssetsPath}/outputTestGraph.json", serializer.Serialize(extension));
            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            var frameCount = 0;

            // Test takes place over onStart, the first onTick, and the second onTick
            // Could implement as a timeout general thing where we look for _testSuccessful unless it runs too long.
            while(frameCount < 2)
            {
                Debug.Log($"Tick on frame {frameCount}");
                eng.Tick();
                frameCount++;
                yield return null;
            }

            Assert.IsTrue(_testSuccessful);
        }

        private static Graph CreateOnTickTestGraph()
        {
            var g = CreateGraphForTest();

            var onStartNodeTriggeredVariable = g.AddVariable("onStartNodeTriggered", false);
            var onStartNodeTriggeredVariableIndex = g.IndexOfVariable(onStartNodeTriggeredVariable);

            var start = g.CreateNode("event/onStart");
            var hasStartedSet = g.CreateNode("variable/set");
            var branch = g.CreateNode("flow/branch");
            var failStart = g.CreateNode("event/send");
            var failLog = g.CreateNode("debug/log");
            var onTick = g.CreateNode("event/onTick");
            var complete = g.CreateNode("event/send");
            var startAnd = g.CreateNode("math/and");
            var isTimeSinceStartNaN = g.CreateNode("math/isnan");
            var isTimeSinceLastTickNaN = g.CreateNode("math/isnan");

            failStart.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);
            failLog.AddConfiguration(ConstStrings.MESSAGE, "Both timeSinceStart and timeSinceLastTick should be NaN before onTick activates its first out flow. timeSinceStart: {timeSinceStart}, timeSinceLastTick: {timeSinceLastTick}");
            failLog.AddConnectedValue(ConstStrings.TIME_SINCE_START, onTick, ConstStrings.TIME_SINCE_START);
            failLog.AddConnectedValue(ConstStrings.TIME_SINCE_LAST_TICK, onTick, ConstStrings.TIME_SINCE_LAST_TICK);

            hasStartedSet.AddConfiguration(ConstStrings.VARIABLE, onStartNodeTriggeredVariableIndex);
            hasStartedSet.AddValue(ConstStrings.VALUE, true);

            start.AddFlow(hasStartedSet);
            hasStartedSet.AddFlow(branch);

            branch.AddFlow(failLog, ConstStrings.FALSE);
            branch.AddConnectedValue(ConstStrings.CONDITION, startAnd);
            failLog.AddFlow(failStart);

            startAnd.AddConnectedValue(ConstStrings.A, isTimeSinceStartNaN);
            startAnd.AddConnectedValue(ConstStrings.B, isTimeSinceLastTickNaN);

            isTimeSinceStartNaN.AddConnectedValue(ConstStrings.A, onTick, ConstStrings.TIME_SINCE_START);
            isTimeSinceLastTickNaN.AddConnectedValue(ConstStrings.A, onTick, ConstStrings.TIME_SINCE_LAST_TICK);

            complete.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);

            var hasStartedGet = g.CreateNode("variable/get");
            var hasStartedBranch = g.CreateNode("flow/branch");
            var onStartNotTriggeredBeforeOnTickLog = g.CreateNode("debug/log");
            var onStartNotTriggeredBeforeOnTickFail = g.CreateNode("event/send");

            onStartNotTriggeredBeforeOnTickFail.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);
            onStartNotTriggeredBeforeOnTickLog.AddConfiguration(ConstStrings.MESSAGE, "onTick activated its out flow before onStart did.");

            onTick.AddFlow(hasStartedBranch);
            hasStartedBranch.AddConnectedValue(ConstStrings.CONDITION, hasStartedGet);
            hasStartedBranch.AddFlow(onStartNotTriggeredBeforeOnTickLog, ConstStrings.FALSE);
            onStartNotTriggeredBeforeOnTickLog.AddFlow(onStartNotTriggeredBeforeOnTickFail);

            hasStartedGet.AddConfiguration(ConstStrings.VARIABLE, onStartNodeTriggeredVariableIndex);

            var isFirstTickVariable = g.AddVariable("isFirstTick", true);
            var isFirstTickVariableIndex = g.IndexOfVariable(isFirstTickVariable);

            var isFirstTickBranch = g.CreateNode("flow/branch");
            var isFirstTickGet = g.CreateNode("variable/get");
            var isFirstTickSet = g.CreateNode("variable/set");
            isFirstTickGet.AddConfiguration(ConstStrings.VARIABLE, isFirstTickVariableIndex);
            isFirstTickSet.AddConfiguration(ConstStrings.VARIABLE, isFirstTickVariableIndex);

            hasStartedBranch.AddFlow(isFirstTickBranch, ConstStrings.TRUE);
            isFirstTickBranch.AddConnectedValue(ConstStrings.CONDITION, isFirstTickGet);

            var firstTickBranch = g.CreateNode("flow/branch");
            var firstTickAnd = g.CreateNode("math/and");
            var firstTickIsNaN = g.CreateNode("math/isnan");
            var firstTickEq = g.CreateNode("math/eq");

            isFirstTickBranch.AddFlow(firstTickBranch, ConstStrings.TRUE);
            firstTickBranch.AddConnectedValue(ConstStrings.CONDITION, firstTickAnd);
            firstTickAnd.AddConnectedValue(ConstStrings.A, firstTickIsNaN);
            firstTickAnd.AddConnectedValue(ConstStrings.B, firstTickEq);

            firstTickEq.AddConnectedValue(ConstStrings.A, onTick, ConstStrings.TIME_SINCE_START);
            firstTickEq.AddValue(ConstStrings.B, 0f);
            firstTickIsNaN.AddConnectedValue(ConstStrings.A, onTick, ConstStrings.TIME_SINCE_LAST_TICK);

            var firstTickFailLog = g.CreateNode("debug/log");
            var firstTickFail = g.CreateNode("event/send");

            firstTickFail.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);

            firstTickFailLog.AddConfiguration(ConstStrings.MESSAGE, "On the first tick timeSinceStart should be 0 and timeSinceLastTick should be NaN. timeSinceStart: {timeSinceStart}, timeSinceLastTick: {timeSinceLastTick}");
            firstTickFailLog.AddConnectedValue(ConstStrings.TIME_SINCE_START, onTick, ConstStrings.TIME_SINCE_START);
            firstTickFailLog.AddConnectedValue(ConstStrings.TIME_SINCE_LAST_TICK, onTick, ConstStrings.TIME_SINCE_LAST_TICK);

            firstTickFailLog.AddFlow(firstTickFail);

            isFirstTickSet.AddValue(ConstStrings.VALUE, false);
            firstTickBranch.AddFlow(isFirstTickSet, ConstStrings.TRUE);
            firstTickBranch.AddFlow(firstTickFailLog, ConstStrings.FALSE);

            var secondTickBranch = g.CreateNode("flow/branch");
            var secondTickAnd = g.CreateNode("math/and");
            var secondTickGtB = g.CreateNode("math/gt");
            var secondTickGtA = g.CreateNode("math/gt");

            isFirstTickBranch.AddFlow(secondTickBranch, ConstStrings.FALSE);

            secondTickBranch.AddConnectedValue(ConstStrings.CONDITION, secondTickAnd);

            secondTickAnd.AddConnectedValue(ConstStrings.A, secondTickGtA);
            secondTickAnd.AddConnectedValue(ConstStrings.B, secondTickGtB);

            secondTickGtA.AddConnectedValue(ConstStrings.A, onTick, ConstStrings.TIME_SINCE_START);
            secondTickGtA.AddValue(ConstStrings.B, 0f);
            secondTickGtB.AddConnectedValue(ConstStrings.A, onTick, ConstStrings.TIME_SINCE_LAST_TICK);
            secondTickGtB.AddValue(ConstStrings.B, 0f);

            secondTickBranch.AddFlow(complete, ConstStrings.TRUE);

            var secondTickFailLog = g.CreateNode("debug/log");
            var secondTickFail = g.CreateNode("event/send");

            secondTickFail.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);

            secondTickFailLog.AddConfiguration(ConstStrings.MESSAGE, "On ticks after the first both timeSinceStart should be valid and above 0. timeSinceStart: {timeSinceStart}, timeSinceLastTick: {timeSinceLastTick}");
            secondTickFailLog.AddConnectedValue(ConstStrings.TIME_SINCE_START, onTick, ConstStrings.TIME_SINCE_START);
            secondTickFailLog.AddConnectedValue(ConstStrings.TIME_SINCE_LAST_TICK, onTick, ConstStrings.TIME_SINCE_LAST_TICK);
            secondTickFailLog.AddFlow(secondTickFail);

            secondTickBranch.AddFlow(secondTickFailLog, ConstStrings.FALSE);
            return g;
        }
    }
}