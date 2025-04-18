using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public static readonly ISubGraph[] subGraphs = new ISubGraph[]
        {
            new ApproximatelySubGraph(0.001f),
            new EqualSubGraph(),
            new IsNaNSubGraph(),
            new IsInfSubGraph()
        };

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

        protected static void TestNode<V>(string nodeName, V expected, ComparisonType subGraphType = ComparisonType.Equals) => TestNode(nodeName, new V[0], expected, subGraphType);
        protected static void TestNode<T, V>(string nodeName, T a, V expected, ComparisonType subGraphType = ComparisonType.Equals) => TestNode(nodeName, new T[1] { a }, expected, subGraphType);
        protected static void TestNode<T, V>(string nodeName, T a, T b, V expected, ComparisonType subGraphType = ComparisonType.Equals) => TestNode(nodeName, new T[2] { a, b }, expected, subGraphType);
        protected static void TestNode<T, V>(string nodeName, T a, T b, T c, V expected, ComparisonType subGraphType = ComparisonType.Equals) => TestNode(nodeName, new T[3] { a, b, c }, expected, subGraphType);
        protected static void TestNode<T, V>(string nodeName, T a, T b, T c, T d, V expected, ComparisonType subGraphType = ComparisonType.Equals) => TestNode(nodeName, new T[4] { a, b, c, d }, expected, subGraphType);

        protected static void TestNodeWithAllFloatNInputVariants(string nodeName, float4 a, float4 expected, ComparisonType subGraphType = ComparisonType.Approximately)
        {
            TestNode(nodeName, new float[1] { a.x }, expected.x, subGraphType);
            TestNode(nodeName, new float[1] { a.y }, expected.y, subGraphType);
            TestNode(nodeName, new float[1] { a.z }, expected.z, subGraphType);
            TestNode(nodeName, new float[1] { a.w }, expected.w, subGraphType);

            TestNode(nodeName, new float2[1] { new float2(a.x, a.y) }, expected.xy, subGraphType);
            TestNode(nodeName, new float2[1] { new float2(a.z, a.w) }, expected.zw, subGraphType);

            TestNode(nodeName, new float3[1] { new float3(a.x, a.y, a.z) }, expected.xyz, subGraphType);
            TestNode(nodeName, new float3[1] { new float3(a.y, a.z, a.w) }, expected.yzw, subGraphType);

            TestNode(nodeName, new float4[1] { a }, expected, subGraphType);
        }

        protected static void TestNodeWithAllFloatNInputVariants(string nodeName, float4 a, float4 b, float4 expected, ComparisonType subGraphType = ComparisonType.Approximately)
        {
            TestNode(nodeName, new float[2] { a.x, b.x }, expected.x, subGraphType);
            TestNode(nodeName, new float[2] { a.y, b.y }, expected.y, subGraphType);
            TestNode(nodeName, new float[2] { a.z, b.z }, expected.z, subGraphType);
            TestNode(nodeName, new float[2] { a.w, b.w }, expected.w, subGraphType);

            TestNode(nodeName, new float2[2] { new float2(a.x, a.y), new float2(b.x, b.y) }, expected.xy, subGraphType);
            TestNode(nodeName, new float2[2] { new float2(a.z, a.w), new float2(b.z, b.w) }, expected.zw, subGraphType);

            TestNode(nodeName, new float3[2] { new float3(a.x, a.y, a.z), new float3(b.x, b.y, b.z) }, expected.xyz, subGraphType);
            TestNode(nodeName, new float3[2] { new float3(a.y, a.z, a.w), new float3(b.y, b.z, b.w) }, expected.yzw, subGraphType);

            TestNode(nodeName, new float4[2] { a, b }, expected, subGraphType);
        }

        protected static void TestNodeWithAllFloatNInputVariants(string nodeName, float4 a, float4 b, float4 c, float4 expected, ComparisonType subGraphType = ComparisonType.Approximately)
        {
            TestNode(nodeName, new float[3] { a.x, b.x, c.x }, expected.x, subGraphType);
            TestNode(nodeName, new float[3] { a.y, b.y, c.y }, expected.y, subGraphType);
            TestNode(nodeName, new float[3] { a.z, b.z, c.z }, expected.z, subGraphType);
            TestNode(nodeName, new float[3] { a.w, b.w, c.w }, expected.w, subGraphType);

            TestNode(nodeName, new float2[3]
            {
                new float2(a.x, a.y),
                new float2(b.x, b.y),
                new float2(c.x, c.y)
            }, expected.xy, subGraphType);

            TestNode(nodeName, new float2[3]
            {
                new float2(a.z, a.w),
                new float2(b.z, b.w),
                new float2(c.z, c.w)
            }, expected.zw, subGraphType);

            TestNode(nodeName, new float3[3]
            {
                new float3(a.x, a.y, a.z),
                new float3(b.x, b.y, b.z),
                new float3(c.x, c.y, c.z)
            }, expected.xyz, subGraphType);

            TestNode(nodeName, new float3[3]
            {
                new float3(a.y, a.z, a.w),
                new float3(b.y, b.z, b.w),
                new float3(c.y, c.z, c.w)
            }, expected.yzw, subGraphType);

            TestNode(nodeName, new float4[3] { a, b, c }, expected, subGraphType);
        }

        protected static (Graph, Node) CreateSelfContainedTestGraph(string nodeStr, Dictionary<string, Value> values, Dictionary<string, IProperty> expectedResults, ComparisonType subGraphType)
        {
            Graph g = CreateGraphForTest();

            g.AddEvent("Pass");
            g.AddEvent("Fail");

            var opNode = g.CreateNode(nodeStr);
            var subGraph = subGraphs[(int)subGraphType];

            foreach (var value in values)
            {
                opNode.AddValue(value.Key, value.Value);
            }

            GenerateGraphByExpectedValueType(expectedResults, g, opNode, subGraph);

            //GraphSerializer serializer = new(Newtonsoft.Json.Formatting.Indented);
            //var extension = new KHR_interactivity()
            //{
            //    graphs = new List<Graph>() { g },
            //    defaultGraphIndex = 0
            //};
            //File.WriteAllText($"{Application.streamingAssetsPath}/outputTestGraph_{expectedResults[ConstStrings.VALUE].GetTypeSignature()}.json", serializer.Serialize(extension));

            return (g, opNode);
        }

        private static void GenerateGraphByExpectedValueType(Dictionary<string, IProperty> expectedResults, Graph g, Node opNode, ISubGraph subGraph)
        {
            Value value;
            Node node;

            foreach (var expected in expectedResults)
            {
                switch (expected.Value)
                {
                    default:
                        CreateSingleValueTestSubGraph(g, opNode, expected.Key, expected.Value, subGraph);
                        break;
                    case Property<float2>:
                        node = CreateExtractNode(g, "math/extract2", opNode, out value, out node, expected);
                        var f2Val = ((Property<float2>)expected.Value).value;
                        for (int i = 0; i < 2; i++)
                        {
                            CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[i], f2Val[i], subGraph);
                        }
                        break;
                    case Property<float3>:
                        node = CreateExtractNode(g, "math/extract3", opNode, out value, out node, expected);
                        var f3Val = ((Property<float3>)expected.Value).value;
                        for (int i = 0; i < 3; i++)
                        {
                            CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[i], f3Val[i], subGraph);
                        }
                        break;
                    case Property<float4>:
                        node = CreateExtractNode(g, "math/extract4", opNode, out value, out node, expected);
                        var f4Val = ((Property<float4>)expected.Value).value;
                        for (int i = 0; i < 4; i++)
                        {
                            CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[i], f4Val[i], subGraph);
                        }
                        break;
                    case Property<float2x2>:
                        node = CreateExtractNode(g, "math/extract2x2", opNode, out value, out node, expected);
                        var f2x2Val = ((Property<float2x2>)expected.Value).value;
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[0], f2x2Val.c0.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[1], f2x2Val.c0.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[2], f2x2Val.c1.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[3], f2x2Val.c1.y, subGraph);
                        break;

                    case Property<float3x3>:
                        node = CreateExtractNode(g, "math/extract3x3", opNode, out value, out node, expected);
                        var f3x3Val = ((Property<float3x3>)expected.Value).value;
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[0], f3x3Val.c0.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[1], f3x3Val.c0.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[2], f3x3Val.c0.z, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[3], f3x3Val.c1.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[4], f3x3Val.c1.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[5], f3x3Val.c1.z, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[6], f3x3Val.c2.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[7], f3x3Val.c2.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[8], f3x3Val.c2.z, subGraph);
                        break;

                    case Property<float4x4>:
                        node = CreateExtractNode(g, "math/extract4x4", opNode, out value, out node, expected);
                        var f4x4Val = ((Property<float4x4>)expected.Value).value;
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[0], f4x4Val.c0.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[1], f4x4Val.c0.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[2], f4x4Val.c0.z, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[3], f4x4Val.c0.w, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[4], f4x4Val.c1.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[5], f4x4Val.c1.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[6], f4x4Val.c1.z, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[7], f4x4Val.c1.w, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[8], f4x4Val.c2.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[9], f4x4Val.c2.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[10], f4x4Val.c2.z, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[11], f4x4Val.c2.w, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[12], f4x4Val.c3.x, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[13], f4x4Val.c3.y, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[14], f4x4Val.c3.z, subGraph);
                        CreateSingleValueTestSubGraph(g, node, ConstStrings.Numbers[15], f4x4Val.c3.w, subGraph);
                        break;

                }
            }

            static Node CreateExtractNode(Graph g, string nodeName, Node opNode, out Value value, out Node node, KeyValuePair<string, IProperty> expected)
            {
                node = g.CreateNode(nodeName);
                value = node.AddValue(ConstStrings.A, 0);
                value.TryConnectToSocket(opNode, expected.Key);

                return node;
            }
        }

        private static void CreateSingleValueTestSubGraph<T>(Graph g, Node outNode, string outSocket, T expected, ISubGraph subGraph)
        {
            CreateSingleValueTestSubGraph(g, outNode, outSocket, (IProperty)new Property<T>(expected), subGraph);
        }

        private static void CreateSingleValueTestSubGraph(Graph g, Node outNode, string outSocket, IProperty expected, ISubGraph subGraph)
        {
            var onStart = g.CreateNode("event/onStart");
            var pass = g.CreateNode("event/send");
            var fail = g.CreateNode("event/send");
            var failLog = g.CreateNode("debug/log");

            pass.AddConfiguration(ConstStrings.EVENT, 0);
            fail.AddConfiguration(ConstStrings.EVENT, 1);

            failLog.AddConfiguration(ConstStrings.MESSAGE, $"Output {outSocket}" + ", Expected: {expected}, Actual: {actual}");
            (var subIn, var subOut) = subGraph.CreateSubGraph(g);

            var subAValue = subIn.AddValue(ConstStrings.A, 0);
            subAValue.TryConnectToSocket(outNode, outSocket);

            if (subGraph.hasBValue) subIn.AddValue(ConstStrings.B, expected);

            onStart.AddFlow(ConstStrings.OUT, subOut, ConstStrings.IN);

            subOut.AddFlow(ConstStrings.TRUE, pass, ConstStrings.IN);
            subOut.AddFlow(ConstStrings.FALSE, failLog, ConstStrings.IN);

            failLog.AddFlow(ConstStrings.OUT, fail, ConstStrings.IN);
            failLog.AddValue(ConstStrings.EXPECTED, expected);
            var failLogActualValue = failLog.AddValue(ConstStrings.ACTUAL, 0);
            failLogActualValue.TryConnectToSocket(outNode, outSocket);
        }

        protected static void TestNode(string nodeName, Dictionary<string, Value> inputValues, Dictionary<string, IProperty> expectedValues, ComparisonType subGraphType = ComparisonType.Equals)
        {
            var (graph, node) = CreateSelfContainedTestGraph(nodeName, inputValues, expectedValues, subGraphType);

            var eng = CreateBehaviourEngineForGraph(graph, OnCustomEventFired, startPlayback: true);

            void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
            {
                if(eventIndex == 1) Assert.Fail("Fail event was triggered by this test.");
            }
        }

        private static void TestNode<T, V>(string nodeName, T[] inputValues, V expected, ComparisonType subGraphType = ComparisonType.Equals)
        {
            Dictionary<string, Value> inputs = new();
            Dictionary<string, IProperty> expectedOutputs = new();

            for (int i = 0; i < inputValues.Length; i++)
            {
                inputs.Add(ConstStrings.Letters[i], new Value()
                {
                    id = ConstStrings.Letters[i],
                    property = new Property<T>(inputValues[i])
                });
            }

            expectedOutputs.Add(ConstStrings.VALUE, new Property<V>(expected));

            TestNode(nodeName, inputs, expectedOutputs, subGraphType);
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