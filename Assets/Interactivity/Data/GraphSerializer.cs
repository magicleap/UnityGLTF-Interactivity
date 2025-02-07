using Newtonsoft.Json;

namespace UnityGLTF.Interactivity
{
    public class GraphSerializer
    {
        private readonly JsonSerializerSettings _serializationSettings;
        private readonly JsonSerializerSettings _deserializerSettings;

        public GraphSerializer(Formatting formatting = Formatting.None)
        {
            _serializationSettings = new JsonSerializerSettings
            {
                Formatting = formatting,
                Converters =
                {
                    new GraphConverter(),
                },
            };

            _deserializerSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    new GraphConverter(),
                },
            };
        }

        public string Serialize(Graph graph)
        {
            return JsonConvert.SerializeObject(graph, _serializationSettings);
        }

        public Graph Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Graph>(json, _deserializerSettings);
        }
    }
}