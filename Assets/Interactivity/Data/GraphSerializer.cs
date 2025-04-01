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

        public string Serialize(KHR_ExtensionGraph  extensionData)
        {
            return JsonConvert.SerializeObject(extensionData, _serializationSettings);
        }

        public KHR_ExtensionGraph Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<KHR_ExtensionGraph>(json, _deserializerSettings);
        }
    }
}