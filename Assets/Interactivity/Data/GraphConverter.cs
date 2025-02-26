using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityGLTF.Interactivity
{
    public class GraphConverter : JsonConverter<KHR_interactivity>
    {
        public override void WriteJson(JsonWriter writer, KHR_interactivity value, JsonSerializer serializer)
        {
            // TODO: Fix Serialization for new spec.
            //writer.WriteStartObject();

            //var typeIndexByType = TypesSerializer.GetSystemTypeByIndexDictionary(value);
            //NodesSerializer.WriteJson(writer, value.nodes, typeIndexByType);
            //VariablesSerializer.WriteJson(writer, value.variables, typeIndexByType);
            //EventsSerializer.WriteJson(writer, value.customEvents);
            //TypesSerializer.WriteJson(writer, value.types);
            //writer.WriteEndObject();
        }

        public override KHR_interactivity ReadJson(JsonReader reader, System.Type objectType, KHR_interactivity existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);

            var jGraphs = jObj[ConstStrings.GRAPHS];

            var interactivity = new KHR_interactivity();

            foreach (JObject jGraph in jGraphs)
            {
                interactivity.graphs.Add(GenerateGraph(jGraph));
            }

            interactivity.defaultGraphIndex = jObj[ConstStrings.GRAPH].Value<int>();

            return interactivity;
        }

        private static Graph GenerateGraph(JObject jObj)
        {
            var types = TypesDeserializer.GetTypes(jObj);
            var systemTypes = TypesDeserializer.GetSystemTypes(types);
            var variables = VariablesDeserializer.GetVariables(jObj, systemTypes);
            var events = EventsDeserializer.GetEvents(jObj);
            var declarations = DeclarationsDeserializer.GetDeclarations(jObj, systemTypes);
            var nodes = NodesDeserializer.GetNodes(jObj, systemTypes, declarations);

            return new Graph()
            {
                types = types,
                variables = variables,
                customEvents = events,
                nodes = nodes,
                declarations = declarations,
                systemTypes = systemTypes
            };
        }
    }
}