using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Interactivity;
using UnityGLTF.Loader;

public class InteractivityTestsHelpers
{
    protected float4 tv1 = new float4(34.0f, 41.0f, 30.0f, 70.0f);
    protected float4 tv2 = new float4(30.0f, 40.0f, 50.0f, 60.0f);

    protected float4 tv3 = new float4(92.0f, 43.0f, 59.0f, 90.0f);

    protected float4 tv4 = new float4(130.0f, 140.0f, 150.0f, 160.0f);

    protected int tv1i = 10;
    protected int tv2i = 20;

    protected bool tv1b = true;
    protected bool tv2b = false;

    private const string _defaultTestNode = "math/supereq";
    private string _testNode = _defaultTestNode;

    protected class CompareFunc : IDisposable
    {
        private InteractivityTestsHelpers _helper;
        public CompareFunc(InteractivityTestsHelpers h, string cmp)
        {
            _helper = h;
            _helper._testNode = cmp;
        }

        public void Dispose()
        {
            _helper._testNode = _defaultTestNode;
        }
    }

    protected BehaviourEngine RunTestForGraph(Graph g, GLTFSceneImporter importer, bool startPlayback = true)
    {
        BehaviourEngine eng = new BehaviourEngine(g, importer);

        if(startPlayback)
        {
            eng.StartPlayback();
        }
        return eng;
    }

    protected async Task<GLTFSceneImporter> LoadTestModel(string modelName, Action<GameObject, ExceptionDispatchInfo, GLTFSceneImporter>  onLoadComplete = null)
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

    protected (Graph, Node) CreateOperationGraph<T, TRes>(string nodeStr, T[] values, TRes expectedResult, string outputValueSocket = ConstStrings.VALUE)
    {
        Graph g = new Graph();
        g.AddDefaultTypes();

        var onStartNode = g.CreateNode("event/onStart", Vector2.zero);
        var assertNode = g.CreateNode("debug/assert", Vector2.zero);
        var testNode = g.CreateNode(_testNode, Vector2.zero);

        onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);

        var opNode = g.CreateNode(nodeStr, Vector2.zero);

        Debug.Assert(values.Length <= ConstStrings.Letters.Length);
        for(int i = 0; i < values.Length; i++)
        {
            opNode.AddValue(ConstStrings.Letters[i], values[i]);
        }

        if(testNode.TryGetValueById(ConstStrings.A, out Value a))
        {
             a.TryConnectToSocket(opNode, outputValueSocket);

            if(assertNode.TryGetValueById(ConstStrings.A, out Value aa))
            {
                aa.TryConnectToSocket(testNode, ConstStrings.VALUE);
            }

            assertNode.AddValue(ConstStrings.B, expectedResult);

            if(assertNode.TryGetValueById(ConstStrings.C, out Value c))
            {
                c.TryConnectToSocket(opNode, outputValueSocket);
            }
        }
        testNode.AddValue(ConstStrings.B, expectedResult);

        return (g, opNode);
    }

    protected void TestOperationResultCustomOutput<T, TRes>(string nodeStr, T[] values, TRes expectedResult, string outputValueSocket = ConstStrings.VALUE)
    {
        var (g, n) = CreateOperationGraph(nodeStr, values, expectedResult, outputValueSocket);
        RunTestForGraph(g, null);
    }

    protected void TestOperationResult<T, TRes>(string nodeStr, T[] values, TRes expectedResult)
    {
        var (g, n) = CreateOperationGraph(nodeStr, values, expectedResult);
        RunTestForGraph(g, null);
    }

    protected void TestOperationResultWithCondition<T, TRes>(string nodeStr, T v1, T v2, bool condition, TRes expectedResult)
    {
        var (g, n) = CreateOperationGraph(nodeStr, new T[2] { v1, v2 }, expectedResult);
        n.AddValue(ConstStrings.CONDITION, condition);
        RunTestForGraph(g, null);
    }

    protected void TestOperationResult<T>(string nodeStr, Action<Node> addValues, T expectedResult)
    {
        var (g, n) = CreateOperationGraph(nodeStr, new T[] {}, expectedResult);
        addValues?.Invoke(n);
        RunTestForGraph(g, null);
    }

    protected void TestOperationResult<T>(string nodeStr, T expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{}, expectedResult);
    }

    protected void TestOperationResult<T, TRes>(string nodeStr, T val1, TRes expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1}, expectedResult);
    }

    protected void TestOperationResultCustomOutput<T, TRes>(string nodeStr, T val1, TRes expectedResult, string outputValueSocket)
    {
        TestOperationResultCustomOutput(nodeStr, new T[]{val1}, expectedResult, outputValueSocket);
    }

    protected void TestOperationResult<T, TRes>(string nodeStr, T val1, T val2, TRes expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2}, expectedResult);
    }

    protected void TestOperationResult2<T1, T2, TRes>(string nodeStr, T1 val1, T2 val2, TRes expectedResult)
    {
        TestOperationResult(nodeStr, (n)=>
        {
            n.AddValue(ConstStrings.A, val1);
            n.AddValue(ConstStrings.B, val2);
        }, expectedResult);
    }

    protected void TestOperationResult<T, TRes>(string nodeStr, T val1, T val2, T val3, TRes expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2, val3}, expectedResult);
    }

    protected void TestOperationResult<T, TRes>(string nodeStr, T val1, T val2, T val3, T val4, TRes expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2, val3, val4}, expectedResult);
    }

    protected void TestOperationResultAllFloats(string opStr, float4 v, float4 expectedResult)
    {
        TestOperationResult(opStr, v, expectedResult);
        TestOperationResult(opStr, v.xyz, expectedResult.xyz);
        TestOperationResult(opStr, v.xy, expectedResult.xy);
        TestOperationResult(opStr, v.x, expectedResult.x);
    }

    protected void TestOperationResultAllFloats(string opStr, float4 v1, float4 v2, float4 expectedResult)
    {
        TestOperationResult(opStr, v1, v2, expectedResult);
        TestOperationResult(opStr, v1.xyz, v2.xyz, expectedResult.xyz);
        TestOperationResult(opStr, v1.xy, v2.xy, expectedResult.xy);
        TestOperationResult(opStr, v1.x, v2.x, expectedResult.x);
    }

    protected void TestOperationResultAllFloats(string opStr, float4 v1, float4 v2, float4 v3, float4 expectedResult)
    {
        TestOperationResult(opStr, v1, v2, v3, expectedResult);
        TestOperationResult(opStr, v1.xyz, v2.xyz, v3.xyz, expectedResult.xyz);
        TestOperationResult(opStr, v1.xy, v2.xy, v3.xy, expectedResult.xy);
        TestOperationResult(opStr, v1.x, v2.x, v3.x, expectedResult.x);
    }


    protected void TestMathOpAllFloats1op(string opStr, float4 expectedResult)
    {
        TestOperationResultAllFloats(opStr, tv1, expectedResult);
    }

    protected void TestMathOpAllFloats2op(string opStr, float4 expectedResult)
    {
        TestOperationResultAllFloats(opStr, tv1, tv2, expectedResult);
    }

    protected void TestMathOpAllFloats3op(string opStr, float4 expectedResult)
    {
        TestOperationResultAllFloats(opStr, tv1, tv2, tv3, expectedResult);
    }

    protected void TestMathOpInt1op(string opStr, int expectedResult)
    {
        TestOperationResult(opStr, tv1i, expectedResult);
    }

    protected void TestMathOpInt2op(string opStr, int expectedResult)
    {
        TestOperationResult(opStr, tv1i, tv2i, expectedResult);
    }

}
