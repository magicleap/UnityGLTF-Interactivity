using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGLTF.Interactivity;
using UnityGLTF.Interactivity.Materials;
using UnityGLTF.Interactivity.Tests;

public class PointerNodesTests : InteractivityTestsHelpers
{
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

    [UnityTest]
    public IEnumerator TestPointerSet()
    {
        var importer = LoadTestModel("material_pointers_test.gltf");        
            
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
        var importer = LoadTestModel("material_pointers_test.gltf");        
            
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
