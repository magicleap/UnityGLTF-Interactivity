using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Mathematics;
using UnityGLTF;
using UnityGLTF.Interactivity;
using UnityGLTF.Interactivity.Extensions;
using UnityGLTF.Interactivity.Materials;
using UnityGLTF.Interactivity.Tests;
using System.Threading.Tasks;
using UnityGLTF.Loader;
using System;
using System.Runtime.ExceptionServices;
using Newtonsoft.Json.Linq;

public class MyTests
{
    private BehaviourEngine RunTestForGraph(Graph g, GLTFSceneImporter importer)
    {
        BehaviourEngine eng = new BehaviourEngine(g, importer);
        eng.StartPlayback();
        return eng;
    }
    private void TestOperationResult<T>(string nodeStr, T[] values, T expectedResult)
    {
        Graph g = new Graph();
        g.AddDefaultTypes();

        var onStartNode = g.CreateNode("event/onStart", Vector2.zero);
        var assertNode = g.CreateNode("debug/assert", Vector2.zero);
        onStartNode.AddFlow(ConstStrings.OUT, assertNode, ConstStrings.IN);


        var opNode = g.CreateNode(nodeStr, Vector2.zero);

        string[] constStrings = new string[5] {ConstStrings.A, ConstStrings.B, ConstStrings.C, ConstStrings.D, ConstStrings.E};
        Debug.Assert(values.Length <= constStrings.Length);
        for(int i = 0; i < values.Length; i++)
        {
            opNode.AddValue(constStrings[i], values[i]);
        }

        assertNode.AddValue(ConstStrings.B, expectedResult);

        if(assertNode.TryGetValueById(ConstStrings.A, out Value a))
        {
             a.TryConnectToSocket(opNode, ConstStrings.VALUE);
        }

        RunTestForGraph(g, null);
    }

    private void TestOperationResult<T>(string nodeStr, T val1, T expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1}, expectedResult);
    }

    private void TestOperationResult<T>(string nodeStr, T val1, T val2, T expectedResult)
    {
        TestOperationResult(nodeStr, new T[]{val1, val2}, expectedResult);
    }

    private Graph CreatePointerInterpolateGraph<T>(int nodeIndex, string pointer, float duration, T val)
    {
        var graph = new Graph();
        graph.AddDefaultTypes();

        var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
        var pointerIntNode = graph.CreateNode("pointer/interpolate", Vector2.zero);

        onStartNode.AddFlow(ConstStrings.OUT, pointerIntNode, ConstStrings.IN);
        pointerIntNode.AddValue("nodeIndex", nodeIndex);
        pointerIntNode.AddConfiguration("type", new JArray("float"));
        pointerIntNode.AddConfiguration("pointer", new JArray("/materials/{nodeIndex}/" + pointer));

        pointerIntNode.AddValue(ConstStrings.DURATION, duration);
        pointerIntNode.AddValue(ConstStrings.VALUE, val);

        pointerIntNode.AddValue(ConstStrings.P1, new float2(0.2f, 0.2f));
        pointerIntNode.AddValue(ConstStrings.P2, new float2(0.6f, 0.6f));


        return graph;
    }

    float4 tv1 = new float4(10.0f, 20.0f, 30.0f, 40.0f);
    float4 tv2 = new float4(30.0f, 40.0f, 50.0f, 60.0f);

    private int tv1i = 10;
    private int tv2i = 20;

    private void TestMathOpAllFloats1op(string opStr, float4 expectedResult)
    {
        TestOperationResult(opStr, tv1, expectedResult);
        TestOperationResult(opStr, tv1.xyz, expectedResult.xyz);
        TestOperationResult(opStr, tv1.xy, expectedResult.xy);
        TestOperationResult(opStr, tv1.x, expectedResult.x);
    }

    private void TestMathOpAllFloats2op(string opStr, float4 expectedResult)
    {
        TestOperationResult(opStr, tv1, tv2, expectedResult);
        TestOperationResult(opStr, tv1.xyz, tv2.xyz, expectedResult.xyz);
        TestOperationResult(opStr, tv1.xy, tv2.xy, expectedResult.xy);
        TestOperationResult(opStr, tv1.x, tv2.x, expectedResult.x);
    }

    private void TestMathOpInt1op(string opStr, int expectedResult)
    {
        TestOperationResult(opStr, tv1i, expectedResult);
    }

    private void TestMathOpInt2op(string opStr, int expectedResult)
    {
        TestOperationResult(opStr, tv1i, tv2i, expectedResult);
    }


    [Test]
    public void TestAdd()
    {
        TestMathOpAllFloats2op("math/add", tv1 + tv2);
        TestMathOpInt2op("math/add", tv1i + tv2i);
    }

    [Test]
    public void TestAbs()
    {
        TestMathOpAllFloats1op("math/abs", math.abs(tv1));
        TestMathOpInt1op("math/abs", (int)math.abs(tv1i));
    }

    
    private ImporterFactory _importerFactory;
    private ImportOptions _importOptions;

    private async Task<GLTFSceneImporter> LoadModel(string modelName, Action<GameObject, ExceptionDispatchInfo, GLTFSceneImporter>  onLoadComplete = null)
    {
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

    [UnityTest]
    public IEnumerator TestPointerSet()
    {
        _importerFactory = ScriptableObject.CreateInstance<DefaultImporterFactory>();
        _importOptions = new ImportOptions()
        {
            ImportNormals = GLTFImporterNormals.Import,
            ImportTangents = GLTFImporterNormals.Import,
        };

        var importer = LoadModel("material_pointers_test.gltf");        
            
        while(importer.IsCompleted == false)
        {
            yield return null;
        }

        var mtg = new MaterialPointerTestGraph();
        var g = mtg.CreateTestGraph("alphaCutoff", "float", 0.67f);

        RunTestForGraph(g, importer.Result);

        var m = importer.Result.MaterialCache[0];
        float ac = m.UnityMaterialWithVertexColor.GetFloat(MaterialPointers.alphaCutoffHash);
        Debug.Assert(ac == 0.67f);
    }

    [UnityTest]
    public IEnumerator TestPointerInterpolate()
    {
        _importerFactory = ScriptableObject.CreateInstance<DefaultImporterFactory>();
        _importOptions = new ImportOptions()
        {
            ImportNormals = GLTFImporterNormals.Import,
            ImportTangents = GLTFImporterNormals.Import,
        };

        var importer = LoadModel("material_pointers_test.gltf");        
            
        while(importer.IsCompleted == false)
        {
            yield return null;
        }

        float duration = 3.5f;
        float targetValue = 0.72f;
        var g = CreatePointerInterpolateGraph(0, "alphaCutoff", duration, targetValue);

        var eng = RunTestForGraph(g, importer.Result);

        while(duration > 0.0f)
        {
            eng.Tick();
            duration -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // to work in PlayMode
        }

        var m = importer.Result.MaterialCache[0];
        float ac = m.UnityMaterialWithVertexColor.GetFloat(MaterialPointers.alphaCutoffHash);
        Debug.Assert(Mathf.Abs(ac - targetValue) < 0.01f);
    }
}
