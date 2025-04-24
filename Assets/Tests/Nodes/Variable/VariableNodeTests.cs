using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity.Tests
{
    public class VariableNodeTests : NodeTestHelpers
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
        public void TestVariableGetAndSet()
        {
            var g = CreateGraphForTest();

            CreateVariableTestSubGraph(g, false, true);
            CreateVariableTestSubGraph(g, 0, 5);
            CreateVariableTestSubGraph(g, 0f, 10f);
            CreateVariableTestSubGraph(g, new float2(0f, 1f), new float2(2f, 3f));
            CreateVariableTestSubGraph(g, new float3(0f, 1f, 2f), new float3(3f, 4f, 5f));
            CreateVariableTestSubGraph(g, new float4(0f, 1f, 2f, 3f), new float4(4f, 5f, 6f, 7f));
            var float2x2a = new float2x2(new float2(0f, 1f), new float2(2f, 3f));
            var float2x2b = new float2x2(new float2(-1f, -2f), new float2(-3f, -4f));
            CreateVariableTestSubGraph(g, float2x2a, float2x2b);
            var float3x3a = new float3x3(
                new float3(0f, 1f, 2f),
                new float3(3f, 4f, 5f),
                new float3(6f, 7f, 8f)
            );
            var float3x3b = new float3x3(
                new float3(-1f, -2f, -3f),
                new float3(-4f, -5f, -6f),
                new float3(-7f, -8f, -9f)
            );
            CreateVariableTestSubGraph(g, float3x3a, float3x3b);

            var float4x4a = new float4x4(
                new float4(0f, 1f, 2f, 3f),
                new float4(4f, 5f, 6f, 7f),
                new float4(8f, 9f, 10f, 11f),
                new float4(12f, 13f, 14f, 15f)
            );
            var float4x4b = new float4x4(
                new float4(-1f, -2f, -3f, -4f),
                new float4(-5f, -6f, -7f, -8f),
                new float4(-9f, -10f, -11f, -12f),
                new float4(-13f, -14f, -15f, -16f)
            );
            CreateVariableTestSubGraph(g, float4x4a, float4x4b);

            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }

        private void CreateVariableTestSubGraph<T>(Graph g, T initialValue, T setValue)
        {
            var onStartNode = g.CreateNode("event/onStart");
            var set = g.CreateNode("variable/set");

            (var branch, var variableIndex) = CreateCheckEqualitySubGraph(g, initialValue, setValue);

            set.AddConfiguration(ConstStrings.VARIABLE, variableIndex);
            set.AddValue(ConstStrings.VALUE, setValue);

            onStartNode.AddFlow(set);
            set.AddFlow(branch);
        }

        private static (Node branch, int variableIndex) CreateCheckEqualitySubGraph<T>(Graph g, T initialValue, T setValue)
        {
            var variableName = $"{Helpers.GetSignatureBySystemType(typeof(T))}Variable";
            var variable = g.AddVariable(variableName, initialValue);
            var variableIndex = g.IndexOfVariable(variable);

            var get = g.CreateNode("variable/get");
            var fail = g.CreateNode("event/send");
            var success = g.CreateNode("event/send");
            var branch = g.CreateNode("flow/branch");
            var eq = g.CreateNode("math/eq");
            var failLog = g.CreateNode("debug/log");

            fail.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);
            success.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);

            get.AddConfiguration(ConstStrings.VARIABLE, variableIndex);

            branch.AddConnectedValue(ConstStrings.CONDITION, eq);
            eq.AddConnectedValue(ConstStrings.A, get);
            eq.AddValue(ConstStrings.B, setValue);

            branch.AddFlow(success, ConstStrings.TRUE);
            branch.AddFlow(failLog, ConstStrings.FALSE);

            failLog.AddConfiguration(ConstStrings.MESSAGE, $"Variable \"{variableName}\" " + "Expected: {expected}, Actual: {actual}");
            failLog.AddValue(ConstStrings.EXPECTED, setValue);
            failLog.AddConnectedValue(ConstStrings.ACTUAL, get);

            failLog.AddFlow(fail);

            return (branch, variableIndex);
        }

        [Test]
        public void TestVariableSetMultiple()
        {
            var g = CreateGraphForTest();

            var onStartNode = g.CreateNode("event/onStart");
            var set = g.CreateNode("variable/setMultiple");
            var sequence = g.CreateNode("flow/sequence");

            onStartNode.AddFlow(set);
            set.AddFlow(sequence);

            var expectedf = 10f;
            var expectedi = 5;
            var expectedb = true;
            var expected2 = new float2(9f, 8f);
            var expected3 = new float3(5f, 4f, 3f);
            var expected4 = new float4(9f, 8f, 7f, 6f);
            var expected2x2 = new float2x2(new float2(4f, 3f), new float2(2f, 1f));
            var expected3x3 = new float3x3(new float3(13f, 12f, 11f), new float3(10f, 9f, 8f), new float3(7f, 6f, 5f));
            var expected4x4 = new float4x4(new float4(29f, 28f, 27f, 26f), new float4(25f, 24f, 23f, 22f), new float4(21f, 20f, 19f, 18f), new float4(17f, 16f, 15f, 14f));

            (var boolBranch, var boolIndex) = CreateCheckEqualitySubGraph(g, false, expectedb);
            (var intBranch, var intIndex) = CreateCheckEqualitySubGraph(g, 0, expectedi);
            (var floatBranch, var floatIndex) = CreateCheckEqualitySubGraph(g, 0f, expectedf);
            (var float2Branch, var float2Index) = CreateCheckEqualitySubGraph(g, new float2(1f, 2f), expected2);
            (var float3Branch, var float3Index) = CreateCheckEqualitySubGraph(g, new float3(3f, 4f, 5f), expected3);
            (var float4Branch, var float4Index) = CreateCheckEqualitySubGraph(g, new float4(6f, 7f, 8f, 9f), expected4);
            (var float2x2Branch, var float2x2Index) = CreateCheckEqualitySubGraph(g, new float2x2(new float2(1f, 2f), new float2(3f, 4f)), expected2x2);
            (var float3x3Branch, var float3x3Index) = CreateCheckEqualitySubGraph(g, new float3x3(new float3(5f, 6f, 7f), new float3(8f, 9f, 10f), new float3(11f, 12f, 13f)), expected3x3);
            (var float4x4Branch, var float4x4Index) = CreateCheckEqualitySubGraph(g,
                new float4x4(
                    new float4(14f, 15f, 16f, 17f),
                    new float4(18f, 19f, 20f, 21f),
                    new float4(22f, 23f, 24f, 25f),
                    new float4(26f, 27f, 28f, 29f)
                ), expected4x4
            );

            sequence.AddFlow(boolBranch, ConstStrings.Numbers[0]);
            sequence.AddFlow(intBranch, ConstStrings.Numbers[1]);
            sequence.AddFlow(floatBranch, ConstStrings.Numbers[2]);
            sequence.AddFlow(float2Branch, ConstStrings.Numbers[3]);
            sequence.AddFlow(float3Branch, ConstStrings.Numbers[4]);
            sequence.AddFlow(float4Branch, ConstStrings.Numbers[5]);
            sequence.AddFlow(float2x2Branch, ConstStrings.Numbers[6]);
            sequence.AddFlow(float3x3Branch, ConstStrings.Numbers[7]);
            sequence.AddFlow(float4x4Branch, ConstStrings.Numbers[8]);

            set.AddValue(ConstStrings.Numbers[0], expectedb);
            set.AddValue(ConstStrings.Numbers[1], expectedi);
            set.AddValue(ConstStrings.Numbers[2], expectedf);
            set.AddValue(ConstStrings.Numbers[3], expected2);
            set.AddValue(ConstStrings.Numbers[4], expected3);
            set.AddValue(ConstStrings.Numbers[5], expected4);
            set.AddValue(ConstStrings.Numbers[6], expected2x2);
            set.AddValue(ConstStrings.Numbers[7], expected3x3);
            set.AddValue(ConstStrings.Numbers[8], expected4x4);

            set.AddConfiguration(ConstStrings.VARIABLES, new int[] {
                boolIndex,
                intIndex,
                floatIndex,
                float2Index,
                float3Index,
                float4Index,
                float2x2Index,
                float3x3Index,
                float4x4Index
            });

            //GraphSerializer serializer = new(Newtonsoft.Json.Formatting.Indented);
            //var extension = new KHR_interactivity()
            //{
            //    graphs = new List<Graph>() { g },
            //    defaultGraphIndex = 0
            //};
            //File.WriteAllText($"{Application.streamingAssetsPath}/outputTestGraph.json", serializer.Serialize(extension));
            var eng = CreateBehaviourEngineForGraph(g, OnCustomEventFired, startPlayback: true);

            Assert.IsTrue(_testSuccessful);
        }
    }
}