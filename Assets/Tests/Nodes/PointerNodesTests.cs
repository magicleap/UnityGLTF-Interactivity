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
    public class PointerNodesTests : NodeTestHelpers
    {
        private (Graph, Node) CreatePointerInterpolateGraph<T>(int nodeIndex, string pointer, float duration, T val)
        {
            var graph = new Graph();
            graph.AddDefaultTypes();

            var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
            var pointerIntNode = graph.CreateNode("pointer/interpolate", Vector2.zero);

            onStartNode.AddFlow(ConstStrings.OUT, pointerIntNode, ConstStrings.IN);
            pointerIntNode.AddValue("nodeIndex", nodeIndex);
            pointerIntNode.AddConfiguration("type", new JArray("float"));
            pointerIntNode.AddConfiguration("pointer", new JArray(pointer));

            pointerIntNode.AddValue(ConstStrings.DURATION, duration);
            pointerIntNode.AddValue(ConstStrings.VALUE, val);

            pointerIntNode.AddValue(ConstStrings.P1, new float2(0.2f, 0.2f));
            pointerIntNode.AddValue(ConstStrings.P2, new float2(0.6f, 0.6f));


            return (graph, pointerIntNode);
        }

        private (Graph, Node) CreateMaterialPointerSetGraph<T>(string pointer, string type, T val)
        {
            var graph = new Graph();
            graph.AddDefaultTypes();

            var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
            var pointerSetNode = graph.CreateNode("pointer/set", Vector2.zero);

            onStartNode.AddFlow(ConstStrings.OUT, pointerSetNode, ConstStrings.IN);
            pointerSetNode.AddValue("nodeIndex", 0);
            pointerSetNode.AddConfiguration("type", new JArray(type));

            pointerSetNode.AddConfiguration("pointer", new JArray(pointer));

            pointerSetNode.AddValue("value", val);

            return (graph, pointerSetNode);
        }

        private (Graph, Node) CreateMaterialPointerGetGraph<T>(string pointer, T val)
        {
            var graph = new Graph();
            graph.AddDefaultTypes();

            var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
            var assertNode = graph.CreateNode("debug/assert", Vector2.zero);
            onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);


            var pointerGetNode = graph.CreateNode("pointer/get", Vector2.zero);
            pointerGetNode.AddValue("nodeIndex", 0);
            pointerGetNode.AddConfiguration("pointer", new JArray(pointer));

            assertNode.AddValue(ConstStrings.B, val);

            if (assertNode.TryGetValueById(ConstStrings.A, out Value a))
            {
                a.TryConnectToSocket(pointerGetNode, ConstStrings.VALUE);
            }

            return (graph, pointerGetNode);
        }

        private IEnumerator TestPointerSet<T>(string pointer, string type, T targetVal) where T : struct
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (importer.IsCompleted == false)
            {
                yield return null;
            }

            if (pointer.StartsWith("/materials") == false)
            {
                pointer = "/materials/{nodeIndex}/" + pointer;
            }

            var (g, n) = CreateMaterialPointerSetGraph(pointer, type, targetVal);

            var eng = CreateBehaviourEngineForGraph(g, null, importer.Result, startPlayback: true);

            var p = eng.pointerResolver.GetPointer(pointer, eng.engineNodes[n]);
            Debug.Assert(p != null);
            Debug.Assert(((Pointer<T>)p).GetValue().Equals(targetVal));
        }

        private IEnumerator TestPointerGet<T>(string pointer, T targetVal) where T : struct
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (importer.IsCompleted == false)
            {
                yield return null;
            }

            if (pointer.StartsWith("/materials") == false)
            {
                pointer = "/materials/{nodeIndex}/" + pointer;
            }

            var (g, n) = CreateMaterialPointerGetGraph(pointer, targetVal);

            var eng = CreateBehaviourEngineForGraph(g, null, importer.Result, startPlayback: false);

            var p = eng.pointerResolver.GetPointer(pointer, eng.engineNodes[n]);
            Debug.Assert(p != null);
            Pointer<T> ptr = (Pointer<T>)p;
            ptr.setter(targetVal);

            eng.StartPlayback();
        }

        [UnityTest]
        public IEnumerator TestPointerSetAlphaCutoff()
        {
            yield return TestPointerSet("alphaCutoff", "float", 0.72f);
        }

        [UnityTest]
        public IEnumerator TestPointerSetIridescence()
        {
            yield return TestPointerSet("extensions/KHR_materials_iridescence/iridescenceFactor", "float", 0.72f);
        }

        [UnityTest]
        public IEnumerator TestPointerGetAlphaCutoff()
        {
            yield return TestPointerGet("alphaCutoff", 0.81f);
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
            var (g, n) = CreatePointerInterpolateGraph(0, pointer, duration, targetValue);

            var eng = CreateBehaviourEngineForGraph(g, null, importer.Result, startPlayback: true);

            while (duration > 0.0f)
            {
                eng.Tick();
                duration -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate(); // to work in PlayMode
            }

            var p = eng.pointerResolver.GetPointer(pointer, eng.engineNodes[n]);
            Debug.Assert(p != null);
            float val = ((Pointer<float>)p).GetValue();
            Debug.Assert(Mathf.Abs(val - targetValue) < 0.01f);
        }

        [UnityTest]
        public IEnumerator TestPointerInterpolateAlphaCutoff()
        {
            yield return TestPointerInterpolateFloat("alphaCutoff", 0.78f);
        }

        private IEnumerator TestTextureTransformValue<T>(string tex, string comp, string type, T targetVal)
        {
            var importer = LoadTestModel("material_pointers_test.gltf");
            while (importer.IsCompleted == false)
            {
                yield return null;
            }

            string pointer = tex + "/extensions/KHR_texture_transform/" + comp;
            var (g, n) = CreateMaterialPointerSetGraph(pointer, type, targetVal);

            var eng = CreateBehaviourEngineForGraph(g, null, importer.Result, startPlayback: true);

            var p = eng.pointerResolver.GetPointer(pointer, eng.engineNodes[n]);
            Debug.Assert(p != null);
            Debug.Assert(((Pointer<T>)p).GetValue().Equals(targetVal));
        }

        private IEnumerator TestTextureTransform(string texPath)
        {
            yield return TestTextureTransformValue(texPath, "offset", "float2", new float2(0.2f, 0.3f));
            yield return TestTextureTransformValue(texPath, "scale", "float2", new float2(0.2f, 0.3f));
            yield return TestTextureTransformValue(texPath, "rotation", "float", 0.6f);
        }

        [UnityTest]
        public IEnumerator TestTextureTransforms()
        {
            yield return TestTextureTransform("/materials/{nodeIndex}/normalTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularColorTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/occlusionTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/emissiveTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/pbrMetallicRoughness/baseColorTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/pbrMetallicRoughness/metallicRoughnessTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatRoughnessTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceThicknessTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenColorTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenRoughnessTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularColorTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_transmission/transmissionTexture");
            yield return TestTextureTransform("/materials/{nodeIndex}/extensions/KHR_materials_volume/thicknessTexture");
        }
    }
}