using System.IO;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class TestGraph : MonoBehaviour
    {
        [SerializeField] private TextAsset _json;
        [SerializeField] private GameObject go;
        private BehaviourEngine engine;

        private void Awake()
        {
            var serializer = new GraphSerializer();

            var extensionData = serializer.Deserialize(_json.text);
            //var graph = CreateGraph();

            SaveGraph(serializer, extensionData);

            //Debug.Log($"Nodes: {graph.nodes.Count}, Variables: {graph.variables.Count}, Types: {graph.types.Count}, Events: {graph.customEvents.Count}");

            //engine = new BehaviourEngine(graph, go);

            //engine.StartPlayback();
            //engine.FireEvent(0);
        }

        private void Update()
        {
            engine.Tick();
        }

        private Graph CreateGraph()
        {
            var graph = new Graph();
            graph.AddDefaultTypes();
            var onStartNode = graph.CreateNode("event/onStart", Vector3.zero);
            var debugLogNode = graph.CreateNode("debug/log", Vector3.zero);

            onStartNode.AddFlow(ConstStrings.OUT, debugLogNode, ConstStrings.IN);
            debugLogNode.AddValue("message", "hello there");

            return graph;
        }

        public void SaveGraph(GraphSerializer serializer, KHR_interactivity extensionData)
        {
            var json = serializer.Serialize(extensionData);

            File.WriteAllText($"{Application.persistentDataPath}/graph.json", json);
        }
    }
}