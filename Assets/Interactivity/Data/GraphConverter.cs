using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityGLTF.Interactivity
{
    public class GraphConverter : JsonConverter<Graph>
    {
        public override void WriteJson(JsonWriter writer, Graph value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var typeIndexByType = TypesSerializer.GetSystemTypeByIndexDictionary(value);
            NodesSerializer.WriteJson(writer, value.nodes, typeIndexByType);
            VariablesSerializer.WriteJson(writer, value.variables, typeIndexByType);
            EventsSerializer.WriteJson(writer, value.customEvents);
            TypesSerializer.WriteJson(writer, value.types);
            writer.WriteEndObject();
        }

        public override Graph ReadJson(JsonReader reader, System.Type objectType, Graph existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);

            var types = TypesDeserializer.GetTypes(jObj);
            var systemTypes = TypesDeserializer.GetSystemTypes(types);
            var variables = VariablesDeserializer.GetVariables(jObj, systemTypes);
            var events = EventsDeserializer.GetEvents(jObj);
            var nodes = NodesDeserializer.GetNodes(jObj, systemTypes);

            return new Graph()
            {
                types = types,
                variables = variables,
                customEvents = events,
                nodes = nodes
            };
        }
    }
}