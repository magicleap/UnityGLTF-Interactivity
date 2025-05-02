using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGLTF.Cache;
using UnityGLTF.Interactivity;
using UnityGLTF.Interactivity.Materials;
using UnityGLTF.Interactivity.Tests;

namespace UnityGLTF.Interactivity.Tests
{
    public partial class PointerNodesTests : NodeTestHelpers
    {
        protected override string _subDirectory => "Pointer";

        private Graph CreatePointerInterpolateGraph<T>(int nodeIndex, string pointer, float duration, T val)
        {
            var graph = new Graph();
            graph.AddDefaultTypes();

            var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
            var pointerIntNode = graph.CreateNode("pointer/interpolate", Vector2.zero);

            onStartNode.AddFlow(pointerIntNode, ConstStrings.OUT, ConstStrings.IN);
            pointerIntNode.AddValue("nodeIndex", nodeIndex);
            pointerIntNode.AddConfiguration("type", new JArray("float"));
            pointerIntNode.AddConfiguration("pointer", new JArray(pointer));

            pointerIntNode.AddValue(ConstStrings.DURATION, duration);
            pointerIntNode.AddValue(ConstStrings.VALUE, val);

            pointerIntNode.AddValue(ConstStrings.P1, new float2(0.2f, 0.2f));
            pointerIntNode.AddValue(ConstStrings.P2, new float2(0.6f, 0.6f));


            return graph;
        }

        private static Graph CreateMaterialPointerSetGraph(string pointer, string type)
        {
            var g = CreateGraphForTest();

            IProperty value = type switch
            {
                "float" => new Property<float>(0.7f),
                "float2" => new Property<float2>(new float2(0.7f, 0.5f)),
                "float3" => new Property<float3>(new float3(0.7f, 0.5f, 0.3f)),
                "float4" => new Property<float4>(new float4(0.7f, 0.5f, 0.3f, 0.2f)),
                _ => throw new InvalidOperationException(),
            };

            Util.Log($"{pointer} with type {type}");

            var typeIndex = g.IndexOfType(type);
            var onStartNode = g.CreateNode("event/onStart");
            var pointerSetNode = g.CreateNode("pointer/set");

            pointerSetNode.AddValue(ConstStrings.NODE_INDEX, 0);
            pointerSetNode.AddConfiguration(ConstStrings.TYPE, typeIndex);
            pointerSetNode.AddConfiguration(ConstStrings.POINTER, pointer);
            pointerSetNode.AddValue(ConstStrings.VALUE, value);

            var get = g.CreateNode("pointer/get");
            get.AddConfiguration(ConstStrings.POINTER, pointer);
            get.AddConfiguration(ConstStrings.TYPE, typeIndex);
            get.AddValue(ConstStrings.NODE_INDEX, 0);

            var branch = g.CreateNode("flow/branch");
            var eq = g.CreateNode("math/eq");

            branch.AddConnectedValue(ConstStrings.CONDITION, eq);

            eq.AddConnectedValue(ConstStrings.A, get);
            eq.AddValue(ConstStrings.B, value);

            var complete = g.CreateNode("event/send");
            var failLog = CreateFailSubGraph(g, $"Get did not return the correct value for pointer {pointer}." + " Expected: {expected}, Actual: {actual}");
            failLog.AddConnectedValue(ConstStrings.ACTUAL, get);
            failLog.AddValue(ConstStrings.EXPECTED, value);
            failLog.AddValue(ConstStrings.NODE_INDEX, 0);

            onStartNode.AddFlow(pointerSetNode);
            pointerSetNode.AddFlow(branch);
            branch.AddFlow(complete, ConstStrings.TRUE);
            branch.AddFlow(failLog, ConstStrings.FALSE);
            complete.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);
            return g;
        }

        [UnityTest]
        public IEnumerator TestMaterialPointers()
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (!importer.IsCompleted)
            {
                yield return null;
            }

            for (int i = 0; i < MATERIAL_POINTERS.Length; i++)
            { 
                QueueTest($"SetAndGetPointer{MATERIAL_POINTERS[i].pointer.Replace('/','_')}", CreateMaterialPointerSetGraph(MATERIAL_POINTERS[i].pointer, MATERIAL_POINTERS[i].type), importer.Result);
            }
        }

        public IEnumerator TestPointerInterpolateFloat(string pointer, float targetValue)
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (importer.IsCompleted == false)
            {
                yield return new WaitForFixedUpdate();
            }

            if (pointer.StartsWith("/materials") == false)
            {
                pointer = "/materials/{nodeIndex}/" + pointer;
            }

            float duration = 3.5f;
            QueueTest(GetCallerName(), CreatePointerInterpolateGraph(0, pointer, duration, targetValue));
        }

        [UnityTest]
        public IEnumerator TestPointerInterpolateAlphaCutoff()
        {
            yield return TestPointerInterpolateFloat("alphaCutoff", 0.78f);
        }
    }
}