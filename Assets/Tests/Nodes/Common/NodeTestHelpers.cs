using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity;
using UnityGLTF.Loader;

namespace UnityGLTF.Interactivity.Tests
{
    public class NodeTestHelpers
    {
        public const string EXPECTED = "expected";

        private static readonly Dictionary<Type, IAssertionByType> assertionsByType = new()
        {
            [typeof(float)] = new FloatAssertion(),
            [typeof(float2)] = new Float2Assertion(),
            [typeof(float3)] = new Float3Assertion(),
            [typeof(float4)] = new Float4Assertion(),
            [typeof(float2x2)] = new Float2x2Assertion(),
            [typeof(float3x3)] = new Float3x3Assertion(),
            [typeof(float4x4)] = new Float4x4Assertion(),
            [typeof(int)] = new IntAssertion(),
            [typeof(int[])] = new IntArrayAssertion(),
            [typeof(bool)] = new BoolAssertion(),
            [typeof(string)] = new StringAssertion(),
        };

        protected static void TestNode<V>(string nodeName, V expected) => TestNode(nodeName, new V[0], expected);
        protected static void TestNode<T, V>(string nodeName, T a, V expected) => TestNode(nodeName, new T[1] { a }, expected);
        protected static void TestNode<T, V>(string nodeName, T a, T b, V expected) => TestNode(nodeName, new T[2] { a, b }, expected);
        protected static void TestNode<T, V>(string nodeName, T a, T b, T c, V expected) => TestNode(nodeName, new T[3] { a, b, c }, expected);
        protected static void TestNode<T, V>(string nodeName, T a, T b, T c, T d, V expected) => TestNode(nodeName, new T[4] { a, b, c, d }, expected);

        protected static void TestNodeWithAllFloatNInputVariants(string nodeName, float4 a, float4 expected)
        {
            TestNode(nodeName, new float[1] { a.x }, expected.x);
            TestNode(nodeName, new float[1] { a.y }, expected.y);
            TestNode(nodeName, new float[1] { a.z }, expected.z);
            TestNode(nodeName, new float[1] { a.w }, expected.w);

            TestNode(nodeName, new float2[1] { new float2(a.x, a.y) }, expected.xy);
            TestNode(nodeName, new float2[1] { new float2(a.z, a.w) }, expected.zw);

            TestNode(nodeName, new float3[1] { new float3(a.x, a.y, a.z) }, expected.xyz);
            TestNode(nodeName, new float3[1] { new float3(a.y, a.z, a.w) }, expected.yzw);

            TestNode(nodeName, new float4[1] { a }, expected);
        }

        protected static void TestNodeWithAllFloatNInputVariants(string nodeName, float4 a, float4 b, float4 expected)
        {
            TestNode(nodeName, new float[2] { a.x, b.x }, expected.x);
            TestNode(nodeName, new float[2] { a.y, b.y }, expected.y);
            TestNode(nodeName, new float[2] { a.z, b.z }, expected.z);
            TestNode(nodeName, new float[2] { a.w, b.w }, expected.w);

            TestNode(nodeName, new float2[2] { new float2(a.x, a.y), new float2(b.x, b.y) }, expected.xy);
            TestNode(nodeName, new float2[2] { new float2(a.z, a.w), new float2(b.z, b.w) }, expected.zw);

            TestNode(nodeName, new float3[2] { new float3(a.x, a.y, a.z), new float3(b.x, b.y, b.z) }, expected.xyz);
            TestNode(nodeName, new float3[2] { new float3(a.y, a.z, a.w), new float3(b.y, b.z, b.w) }, expected.yzw);

            TestNode(nodeName, new float4[2] { a, b }, expected);
        }

        protected static void TestNodeWithAllFloatNInputVariants(string nodeName, float4 a, float4 b, float4 c, float4 expected)
        {
            TestNode(nodeName, new float[3] { a.x, b.x, c.x }, expected.x);
            TestNode(nodeName, new float[3] { a.y, b.y, c.y }, expected.y);
            TestNode(nodeName, new float[3] { a.z, b.z, c.z }, expected.z);
            TestNode(nodeName, new float[3] { a.w, b.w, c.w }, expected.w);

            TestNode(nodeName, new float2[3]
            {
                new float2(a.x, a.y),
                new float2(b.x, b.y),
                new float2(c.x, c.y)
            }, expected.xy);

            TestNode(nodeName, new float2[3]
            {
                new float2(a.z, a.w),
                new float2(b.z, b.w),
                new float2(c.z, c.w)
            }, expected.zw);

            TestNode(nodeName, new float3[3]
            {
                new float3(a.x, a.y, a.z),
                new float3(b.x, b.y, b.z),
                new float3(c.x, c.y, c.z)
            }, expected.xyz);

            TestNode(nodeName, new float3[3]
            {
                new float3(a.y, a.z, a.w),
                new float3(b.y, b.z, b.w),
                new float3(c.y, c.z, c.w)
            }, expected.yzw);

            TestNode(nodeName, new float4[3] { a, b, c }, expected);
        }

        protected static (Graph, Node) CreateOperationGraph(string nodeStr, Dictionary<string, Value> values, Dictionary<string, IProperty> expectedResults)
        {
            Graph g = CreateGraphForTest();

            var onStartNode = g.CreateNode("event/onStart");
            var assertNode = g.CreateNode("event/send");
            var opNode = g.CreateNode(nodeStr);

            foreach(var result in expectedResults)
            {
                g.AddVariable(result.Key, result.Value);
            }

            g.AddEvent("Result");
            assertNode.AddConfiguration(ConstStrings.EVENT, 0);

            onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);

            foreach(var value in values)
            {
                opNode.AddValue(value.Key, value.Value);
            }

            foreach (var result in expectedResults)
            {
                var outputValue = assertNode.AddValue(result.Key, 0);
                outputValue.TryConnectToSocket(opNode, result.Key);
            }

            return (g, opNode);
        }

        protected static (Graph, Node) CreateOperationGraph<T, TRes>(string nodeStr, T[] values, TRes expectedResult, string outputValueSocket = ConstStrings.VALUE)
        {
            Graph g = CreateGraphForTest();

            var onStartNode = g.CreateNode("event/onStart");
            var assertNode = g.CreateNode("event/send");
            var opNode = g.CreateNode(nodeStr);

            g.AddVariable(EXPECTED, expectedResult);
            g.AddEvent("Result");
            assertNode.AddConfiguration(ConstStrings.EVENT, 0);

            onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);

            Assert.IsTrue(values.Length <= ConstStrings.Letters.Length);

            for (int i = 0; i < values.Length; i++)
            {
                opNode.AddValue(ConstStrings.Letters[i], values[i]);
            }

            var outputValue = assertNode.AddValue(ConstStrings.VALUE, 0);
            outputValue.TryConnectToSocket(opNode, ConstStrings.VALUE);

            return (g, opNode);
        }

        protected static void TestNode(string nodeName, Dictionary<string, Value> inputValues, Dictionary<string, IProperty> expectedValues)
        {
            var (graph, node) = CreateOperationGraph(nodeName, inputValues, expectedValues);

            var eng = CreateBehaviourEngineForGraph(graph, OnCustomEventFired, startPlayback: true);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                foreach(var value in outValues)
                {
                    graph.TryGetVariable(value.Key, out Variable expected);
                    var actual = value.Value;

                    var assertion = assertionsByType[actual.GetSystemType()];
                    assertion.AreApproximatelyEqual(expected.property, actual);
                }
            }
        }

        private static void TestNode<T, V>(string nodeName, T[] inputValues, V expected)
        {
            var (graph, node) = CreateOperationGraph(nodeName, inputValues, expected);

            var eng = CreateBehaviourEngineForGraph(graph, OnCustomEventFired, startPlayback: true);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                var expected = ((Property<V>)graph.variables[0].property).value;
                var actual = ((Property<V>)outValues[ConstStrings.VALUE]).value;

                var assertion = assertionsByType[typeof(V)];
                assertion.AreApproximatelyEqual(expected, actual);
            }
        }

        protected static BehaviourEngine CreateBehaviourEngineForGraph(Graph g, Action<int, Dictionary<string, IProperty>> onEventFired, bool startPlayback = true)
        {
            return CreateBehaviourEngineForGraph(g, onEventFired, null, startPlayback);
        }

        protected static BehaviourEngine CreateBehaviourEngineForGraph(Graph g, Action<int, Dictionary<string, IProperty>> onEventFired, GLTFSceneImporter importer, bool startPlayback)
        {
            BehaviourEngine eng = new BehaviourEngine(g, importer);

            if (onEventFired != null)
                eng.onCustomEventFired += onEventFired;

            if (startPlayback)
                eng.StartPlayback();

            return eng;
        }

        protected static async Task<GLTFSceneImporter> LoadTestModel(string modelName, Action<GameObject, ExceptionDispatchInfo, GLTFSceneImporter> onLoadComplete = null)
        {
            ImporterFactory _importerFactory = ScriptableObject.CreateInstance<DefaultImporterFactory>();
            ImportOptions _importOptions = new ImportOptions()
            {
                ImportNormals = GLTFImporterNormals.Import,
                ImportTangents = GLTFImporterNormals.Import,
            };

            var (directory, fileName) = Helpers.GetFilePath(modelName);

            _importOptions.DataLoader = new UnityWebRequestLoader(directory);

            var importer = _importerFactory.CreateSceneImporter(
                modelName,
                _importOptions
            );

            var sceneParent = new GameObject(fileName).transform;

            importer.SceneParent = sceneParent;
            importer.Collider = GLTFSceneImporter.ColliderType.Box;
            importer.MaximumLod = 300;
            importer.Timeout = 8;
            importer.IsMultithreaded = true;
            importer.CustomShaderName = null;

            // for logging progress
            await importer.LoadSceneAsync(
                showSceneObj: true,
                onLoadComplete: (go, e) => onLoadComplete?.Invoke(go, e, importer)
            );

            return importer;
        }

        protected static Graph CreateGraphForTest()
        {
            var graph = new Graph();
            graph.AddDefaultTypes();

            return graph;
        }
    }
}