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
        private const float INTERPOLATION_DURATION = 0.5f;
        private static readonly float2 P1 = new float2(0.42f, 0f);
        private static readonly float2 P2 = new float2(0.52f, 1f);
        protected override string _subDirectory => "Pointer";

        [UnityTest]
        public IEnumerator PointerInterpolateGet_ValuesInGetAreWhatWereInterpolatedTo()
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (!importer.IsCompleted)
            {
                yield return null;
            }

            for (int i = 0; i < MATERIAL_POINTERS.Length; i++)
            { 
                var pointer = MATERIAL_POINTERS[i].pointer.Replace('/', '_');
                QueueTest("pointer/interpolate", $"InterpolateAndGetPointer{pointer}", $"Pointer Interpolate {pointer}", $"Tests that pointer/interpolate and pointer/get work for {pointer}.", CreatePointerInterpolateGraph(MATERIAL_POINTERS[i].pointer, MATERIAL_POINTERS[i].type), importer.Result);
            }
        }

        [UnityTest]
        public IEnumerator PointerSetGet_ValuesInGetAreWhatWereSet()
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (!importer.IsCompleted)
            {
                yield return null;
            }

            for (int i = 0; i < MATERIAL_POINTERS.Length; i++)
            {
                var pointer = MATERIAL_POINTERS[i].pointer.Replace('/', '_');
                QueueTest("pointer/set", $"SetAndGetPointer{pointer}", $"Pointer Set/Get {pointer}", $"Tests that pointer/set and pointer/get work for {pointer}.", CreatePointerSetGraph(MATERIAL_POINTERS[i].pointer, MATERIAL_POINTERS[i].type), importer.Result);
            }
        }

        private Graph CreatePointerInterpolateGraph(string pointer, string type)
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
            var pointerInterpolateNode = g.CreateNode("pointer/interpolate");

            pointerInterpolateNode.AddValue(ConstStrings.NODE_INDEX, 0);
            pointerInterpolateNode.AddConfiguration(ConstStrings.TYPE, typeIndex);
            pointerInterpolateNode.AddConfiguration(ConstStrings.POINTER, pointer);
            pointerInterpolateNode.AddValue(ConstStrings.VALUE, value);
            pointerInterpolateNode.AddValue(ConstStrings.DURATION, INTERPOLATION_DURATION);
            pointerInterpolateNode.AddValue(ConstStrings.P1, P1);
            pointerInterpolateNode.AddValue(ConstStrings.P2, P2);

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

            onStartNode.AddFlow(pointerInterpolateNode);
            pointerInterpolateNode.AddFlow(branch, ConstStrings.DONE);
            branch.AddFlow(complete, ConstStrings.TRUE);
            branch.AddFlow(failLog, ConstStrings.FALSE);
            complete.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);
            return g;
        }

        private static Graph CreatePointerSetGraph(string pointer, string type)
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
    }
}