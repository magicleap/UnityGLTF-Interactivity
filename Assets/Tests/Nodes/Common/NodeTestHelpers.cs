#define GENERATE_TEST_FILES

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGLTF.Interactivity;
using UnityGLTF.Loader;

namespace UnityGLTF.Interactivity.Tests
{
    public abstract class NodeTestHelpers
    {
        protected struct TestData
        {
            public string fileName;
            public Graph graph;
            public float executionTime;
        }

        protected const int FAIL_EVENT_INDEX = 0;
        protected const int COMPLETED_EVENT_INDEX = 1;
        protected const float DEFAULT_MULTI_FRAME_EXECUTION_TIMEOUT = 10f;

        protected bool _testSuccessful = false;
        private GraphSerializer _serializer = new(Newtonsoft.Json.Formatting.Indented);

        protected string _testGraphDirectory;
        protected abstract string _subDirectory { get; }

        protected static readonly List<TestData> _testQueue = new();

        public static readonly ISubGraph[] subGraphs = new ISubGraph[]
        {
            new ApproximatelySubGraph(0.001f),
            new EqualSubGraph(),
            new IsNaNSubGraph(),
            new IsInfSubGraph()
        };

        protected void OnCustomEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dir = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));
            if (!string.IsNullOrWhiteSpace(_subDirectory))
                _testGraphDirectory = Path.Combine(dir, "TestGraphJson", _subDirectory);
            else
                _testGraphDirectory = Path.Combine(dir, "TestGraphJson");

            Directory.CreateDirectory(_testGraphDirectory);
        }

        [SetUp]
        public void SetUp()
        {
            _testQueue.Clear();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach(var test in _testQueue)
            {
                _testSuccessful = false;
#if GENERATE_TEST_FILES
                Task.Run(() =>
                {
                    // Record graph to json file.
                    if (!string.IsNullOrWhiteSpace(test.fileName))
                    {
                        var extension = new KHR_interactivity()
                        {
                            graphs = new List<Graph>() { test.graph },
                            defaultGraphIndex = 0
                        };
                        File.WriteAllText($"{_testGraphDirectory}/{test.fileName}.json", _serializer.Serialize(extension));
                    }
                });
#endif

                // Actually run test.
                var eng = CreateBehaviourEngineForGraph(test.graph, OnCustomEventFired, startPlayback: true);

                while (!_testSuccessful && Time.time < test.executionTime)
                {
                    eng.Tick();
                    yield return null;
                }

                Assert.IsTrue(_testSuccessful);
            }
        }

        protected static void TestNodeWithAllFloatNInputVariants(string testName, string nodeName, float4 a, float4 expected, ComparisonType subGraphType = ComparisonType.Approximately)
        {
            QueueTest($"{testName}-float-x", CreateSelfContainedTestGraph(nodeName, In(a.x), Out(expected.x), subGraphType));
            QueueTest($"{testName}-float-y", CreateSelfContainedTestGraph(nodeName, In(a.y), Out(expected.y), subGraphType));
            QueueTest($"{testName}-float-z", CreateSelfContainedTestGraph(nodeName, In(a.z), Out(expected.z), subGraphType));
            QueueTest($"{testName}-float-w", CreateSelfContainedTestGraph(nodeName, In(a.w), Out(expected.w), subGraphType));

            QueueTest($"{testName}-float2-xy", CreateSelfContainedTestGraph(nodeName, In(new float2(a.x, a.y)), Out(expected.xy), subGraphType));
            QueueTest($"{testName}-float2-zw", CreateSelfContainedTestGraph(nodeName, In(new float2(a.z, a.w)), Out(expected.zw), subGraphType));

            QueueTest($"{testName}-float3-xyz", CreateSelfContainedTestGraph(nodeName, In(new float3(a.x, a.y, a.z)), Out(expected.xyz), subGraphType));
            QueueTest($"{testName}-float3-yzw", CreateSelfContainedTestGraph(nodeName, In(new float3(a.y, a.z, a.w)), Out(expected.yzw), subGraphType));

            QueueTest($"{testName}-float4", CreateSelfContainedTestGraph(nodeName, In(a), Out(expected), subGraphType));
        }

        protected static void TestNodeWithAllFloatNInputVariants(string testName, string nodeName, float4 a, float4 b, float4 expected, ComparisonType subGraphType = ComparisonType.Approximately)
        {
            QueueTest($"{testName}-float-x", CreateSelfContainedTestGraph(nodeName, In(a.x, b.x), Out(expected.x), subGraphType));
            QueueTest($"{testName}-float-y", CreateSelfContainedTestGraph(nodeName, In(a.y, b.y), Out(expected.y), subGraphType));
            QueueTest($"{testName}-float-z", CreateSelfContainedTestGraph(nodeName, In(a.z, b.z), Out(expected.z), subGraphType));
            QueueTest($"{testName}-float-w", CreateSelfContainedTestGraph(nodeName, In(a.w, b.w), Out(expected.w), subGraphType));

            QueueTest($"{testName}-float2-xy", CreateSelfContainedTestGraph(nodeName, In(new float2(a.x, a.y), new float2(b.x, b.y)), Out(expected.xy), subGraphType));
            QueueTest($"{testName}-float2-zw", CreateSelfContainedTestGraph(nodeName, In(new float2(a.z, a.w), new float2(b.z, b.w)), Out(expected.zw), subGraphType));

            QueueTest($"{testName}-float3-xyz", CreateSelfContainedTestGraph(nodeName, In(new float3(a.x, a.y, a.z), new float3(b.x, b.y, b.z)), Out(expected.xyz), subGraphType));
            QueueTest($"{testName}-float3-yzw", CreateSelfContainedTestGraph(nodeName, In(new float3(a.y, a.z, a.w), new float3(b.y, b.z, b.w)), Out(expected.yzw), subGraphType));

            QueueTest($"{testName}-float4", CreateSelfContainedTestGraph(nodeName, In(a, b), Out(expected), subGraphType));
        }


        protected static void TestNodeWithAllFloatNInputVariants(string testName, string nodeName, float4 a, float4 b, float4 c, float4 expected, ComparisonType subGraphType = ComparisonType.Approximately)
        {
            QueueTest($"{testName}-float-x", CreateSelfContainedTestGraph(nodeName, In(a.x, b.x, c.x), Out(expected.x), subGraphType));
            QueueTest($"{testName}-float-y", CreateSelfContainedTestGraph(nodeName, In(a.y, b.y, c.y), Out(expected.y), subGraphType));
            QueueTest($"{testName}-float-z", CreateSelfContainedTestGraph(nodeName, In(a.z, b.z, c.z), Out(expected.z), subGraphType));
            QueueTest($"{testName}-float-w", CreateSelfContainedTestGraph(nodeName, In(a.w, b.w, c.w), Out(expected.w), subGraphType));

            QueueTest($"{testName}-float2-xy", CreateSelfContainedTestGraph(nodeName,
                In(new float2(a.x, a.y), new float2(b.x, b.y), new float2(c.x, c.y)),
                Out(expected.xy), subGraphType));

            QueueTest($"{testName}-float2-zw", CreateSelfContainedTestGraph(nodeName,
                In(new float2(a.z, a.w), new float2(b.z, b.w), new float2(c.z, c.w)),
                Out(expected.zw), subGraphType));

            QueueTest($"{testName}-float3-xyz", CreateSelfContainedTestGraph(nodeName,
                In(new float3(a.x, a.y, a.z), new float3(b.x, b.y, b.z), new float3(c.x, c.y, c.z)),
                Out(expected.xyz), subGraphType));

            QueueTest($"{testName}-float3-yzw", CreateSelfContainedTestGraph(nodeName,
                In(new float3(a.y, a.z, a.w), new float3(b.y, b.z, b.w), new float3(c.y, c.z, c.w)),
                Out(expected.yzw), subGraphType));

            QueueTest($"{testName}-float4", CreateSelfContainedTestGraph(nodeName, In(a, b, c), Out(expected), subGraphType));
        }


        protected static (Graph, Node) CreateSelfContainedTestGraph(string nodeStr, Dictionary<string, Value> values, Dictionary<string, IProperty> expectedResults, ComparisonType subGraphType)
        {
            Graph g = CreateGraphForTest();

            var opNode = g.CreateNode(nodeStr);
            var subGraph = subGraphs[(int)subGraphType];

            foreach (var value in values)
            {
                opNode.AddValue(value.Key, value.Value);
            }

            GenerateGraphByExpectedValueType(expectedResults, g, opNode, subGraph);

            return (g, opNode);
        }

        private static void GenerateGraphByExpectedValueType(Dictionary<string, IProperty> expectedResults, Graph g, Node opNode, ISubGraph subGraph)
        {
            Value value;
            Node node;

            foreach (var expected in expectedResults)
            {
                if (subGraph is EqualSubGraph)
                {
                    CreateSingleValueTestSubGraph(g, opNode, expected.Key, expected.Value, subGraph);
                    continue;
                }

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

            fail.AddConfiguration(ConstStrings.EVENT, FAIL_EVENT_INDEX);
            pass.AddConfiguration(ConstStrings.EVENT, COMPLETED_EVENT_INDEX);

            failLog.AddConfiguration(ConstStrings.MESSAGE, $"Output {outSocket}" + ", Expected: {expected}, Actual: {actual}");
            (var subIn, var subOut) = subGraph.CreateSubGraph(g);

            var subAValue = subIn.AddValue(ConstStrings.A, 0);
            subAValue.TryConnectToSocket(outNode, outSocket);

            if (subGraph.hasBValue) subIn.AddValue(ConstStrings.B, expected);

            onStart.AddFlow(subOut, ConstStrings.OUT, ConstStrings.IN);

            subOut.AddFlow(pass, ConstStrings.TRUE, ConstStrings.IN);
            subOut.AddFlow(failLog, ConstStrings.FALSE, ConstStrings.IN);

            failLog.AddFlow(fail, ConstStrings.OUT, ConstStrings.IN);
            failLog.AddValue(ConstStrings.EXPECTED, expected);
            var failLogActualValue = failLog.AddValue(ConstStrings.ACTUAL, 0);
            failLogActualValue.TryConnectToSocket(outNode, outSocket);
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
            graph.AddEvent("Failed");
            graph.AddEvent("Completed");
            return graph;
        }

        protected static string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }

        protected static void QueueTest(string fileName, Graph graph)
        {
            _testQueue.Add(new TestData()
            {
                fileName = fileName,
                graph = graph,
                executionTime = DEFAULT_MULTI_FRAME_EXECUTION_TIMEOUT
            });
        }

        protected static void QueueTest(string fileName, (Graph graph, Node node) graphTuple)
        {
            _testQueue.Add(new TestData()
            {
                fileName = fileName,
                graph = graphTuple.graph,
                executionTime = DEFAULT_MULTI_FRAME_EXECUTION_TIMEOUT
            });
        }

        protected static Dictionary<string, Value> In<T>(params T[] inputValues)
        {
            Dictionary<string, Value> inputs = new();

            for (int i = 0; i < inputValues.Length; i++)
            {
                inputs.Add(ConstStrings.Letters[i], new Value()
                {
                    id = ConstStrings.Letters[i],
                    property = new Property<T>(inputValues[i])
                });
            }

            return inputs;
        }

        protected static Dictionary<string, IProperty> Out<T>(T expected)
        {
            Dictionary<string, IProperty> outputs = new();

            outputs.Add(ConstStrings.VALUE, new Property<T>(expected));

            return outputs;
        }

        protected static Dictionary<string, IProperty> Out<T>(params KeyValuePair<string, T>[] expectedOutputs)
        {
            Dictionary<string, IProperty> outputs = new();

            for (int i = 0; i < expectedOutputs.Length; i++)
            {
                outputs.Add(expectedOutputs[i].Key, new Property<T>(expectedOutputs[i].Value));
            }

            return outputs;
        }
    }
}