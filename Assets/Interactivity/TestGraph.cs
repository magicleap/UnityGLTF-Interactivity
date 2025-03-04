using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class TestGraph : MonoBehaviour
    {
        [SerializeField] private TextAsset _json;
        [SerializeField] private GameObject go;
        private readonly BehaviourEngine engine;

        private void Awake()
        {
            var serializer = new GraphSerializer();

            var extensionData = serializer.Deserialize(_json.text);
            //var graph = CreateGraph();

            SaveGraph(serializer, extensionData);

            //Util.Log($"Nodes: {graph.nodes.Count}, Variables: {graph.variables.Count}, Types: {graph.types.Count}, Events: {graph.customEvents.Count}");

            //engine = new BehaviourEngine(graph, go);

            //engine.StartPlayback();
            //engine.FireEvent(0);
        }

        private void Update()
        {
            engine.Tick();
        }

        public static Graph CreateTestGraph()
        {
            var graph = new Graph();
            graph.AddDefaultTypes();


            var materialPointers = new (string, string)[]
            {
                ("/materials/{nodeIndex}/alphaCutoff", "float"),
                ("/materials/{nodeIndex}/emissiveFactor", "float3"),
                ("/materials/{nodeIndex}/normalTexture/scale", "float"),
                ("/materials/{nodeIndex}/occlusionTexture/strength", "float"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/baseColorFactor", "float4"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/metallicFactor", "float"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/roughnessFactor", "float"),

                ("/materials/{nodeIndex}/normalTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/normalTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/normalTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/occlusionTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/occlusionTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/occlusionTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/emissiveTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/emissiveTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/emissiveTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/pbrMetallicRoughness/baseColorTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/baseColorTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/baseColorTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/pbrMetallicRoughness/metallicRoughnessTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/metallicRoughnessTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/pbrMetallicRoughness/metallicRoughnessTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatFactor", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatRoughnessFactor", "float"),
                //("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatNormalTexture/scale", "float"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatRoughnessTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatRoughnessTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatRoughnessTexture/extensions/KHR_texture_transform/scale", "float2"),

                //("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatNormalTexture/extensions/KHR_texture_transform/offset", "float2"),
                //("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatNormalTexture/extensions/KHR_texture_transform/rotation", "float"),
                //("/materials/{nodeIndex}/extensions/KHR_materials_clearcoat/clearcoatNormalTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_dispersion/dispersion", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_ior/ior", "float"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceFactor", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceIor", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceThicknessMinimum", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceThicknessMaximum", "float"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceThicknessTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceThicknessTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_iridescence/iridescenceThicknessTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenColorFactor", "float3"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenRoughnessFactor", "float"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenColorTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenColorTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenColorTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenRoughnessTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenRoughnessTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_sheen/sheenRoughnessTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularFactor", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularColorFactor", "float3"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularColorTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularColorTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_specular/specularColorTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_transmission/transmissionFactor", "float"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_transmission/transmissionTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_transmission/transmissionTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_transmission/transmissionTexture/extensions/KHR_texture_transform/scale", "float2"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_volume/thicknessFactor", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_volume/attenuationDistance", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_volume/attenuationColor", "float3"),

                ("/materials/{nodeIndex}/extensions/KHR_materials_volume/thicknessTexture/extensions/KHR_texture_transform/offset", "float2"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_volume/thicknessTexture/extensions/KHR_texture_transform/rotation", "float"),
                ("/materials/{nodeIndex}/extensions/KHR_materials_volume/thicknessTexture/extensions/KHR_texture_transform/scale", "float2"),
            };

            for (int i = 0; i < materialPointers.Length; i++)
            {
                CreatePointerPair(i, graph, materialPointers[i].Item1, materialPointers[i].Item2);
            }

            return graph;
        }

        public void SaveGraph(GraphSerializer serializer, KHR_interactivity extensionData)
        {
            var json = serializer.Serialize(extensionData);

            File.WriteAllText($"{Application.persistentDataPath}/graph.json", json);
        }

        private static void CreatePointerPair(int i, Graph graph, string pointer, string type)
        {
            var onStartNode = graph.CreateNode("event/onStart", new Vector2(0f, 500f * i));
            var pointerSetNode = graph.CreateNode("pointer/set", new Vector2(500f, 500f * i));

            onStartNode.AddFlow(ConstStrings.OUT, pointerSetNode, ConstStrings.IN);
            pointerSetNode.AddValue("nodeIndex", 1);
            pointerSetNode.AddConfiguration("type", new JArray(type));
            pointerSetNode.AddConfiguration("pointer", new JArray(pointer));

            switch (type)
            {
                case "float":
                    pointerSetNode.AddValue("value", 0.75f);
                    break;
                case "float2":
                    pointerSetNode.AddValue("value", new Vector2(0.5f, 0.2f));
                    break;
                case "float3":
                    pointerSetNode.AddValue("value", new Vector3(0.25f, 0.4f, 0.7f));
                    break;
                case "float4":
                    pointerSetNode.AddValue("value", new Vector4(0.25f, 0.4f, 0.7f, 0.5f));
                    break;
                default:
                    break;
            }
        }
    }
}