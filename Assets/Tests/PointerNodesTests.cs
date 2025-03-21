using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private Graph CreatePointerSetGraph<T>(string prop, string type, T val)
    {
        var graph = new Graph();
        graph.AddDefaultTypes();

        var onStartNode = graph.CreateNode("event/onStart", Vector2.zero);
        var pointerSetNode = graph.CreateNode("pointer/set", Vector2.zero);

        onStartNode.AddFlow(ConstStrings.OUT, pointerSetNode, ConstStrings.IN);
        pointerSetNode.AddValue("nodeIndex", 0);
        pointerSetNode.AddConfiguration("type", new JArray(type));
        pointerSetNode.AddConfiguration("pointer", new JArray("/materials/{nodeIndex}/" + prop));

        pointerSetNode.AddValue("value", val);

        return graph;
    }

    private async Task TestPointerSet<T>(string prop, string type, int hash, T targetVal) where T : struct
    {
        var importer = await LoadTestModel("material_pointers_test.gltf");        

        var g = CreatePointerSetGraph(prop, type, targetVal);

        RunTestForGraph(g, importer);

        var m = importer.MaterialCache[0];
        float val = m.UnityMaterialWithVertexColor.GetFloat(hash);
        Debug.Assert(val.Equals(targetVal));
    }

    [UnityTest]
    public IEnumerator TestPointerSetAlphaCutoff()
    {
        Task task = TestPointerSet("alphaCutoff", "float", MaterialPointers.alphaCutoffHash, 0.72f);
        yield return new WaitUntil(() => task.IsCompleted);
    }

    [UnityTest]
    public IEnumerator TestPointerSetIridescence()
    {
        Task task = TestPointerSet("extensions/KHR_materials_iridescence/iridescenceFactor", "float", IridescencePointers.iridescenceFactorHash, 0.72f);
        yield return new WaitUntil(() => task.IsCompleted);
    }

    public IEnumerator TestPointerInterpolateFloat(string prop, int propHash, float targetValue)
    {
        var importer = LoadTestModel("material_pointers_test.gltf");        
        while(importer.IsCompleted == false)
        {
            yield return null;
        }

        float duration = 3.5f;
        var g = CreatePointerInterpolateGraph(0, prop, duration, targetValue);

        var eng = RunTestForGraph(g, importer.Result);

        while(duration > 0.0f)
        {
            eng.Tick();
            duration -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // to work in PlayMode
        }

        var m = importer.Result.MaterialCache[0];

        float val = m.UnityMaterialWithVertexColor.GetFloat(propHash);
        Debug.Assert(Mathf.Abs(val - targetValue) < 0.01f);
    }

    [UnityTest]
    public IEnumerator TestPointerInterpolateAlphaCutoff()
    {
        yield return TestPointerInterpolateFloat("alphaCutoff", MaterialPointers.alphaCutoffHash, 0.78f);
    }
}
