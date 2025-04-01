using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public class GraphConverter : JsonConverter<KHR_ExtensionGraph>
    {
        public override void WriteJson(JsonWriter writer, KHR_ExtensionGraph value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(ConstStrings.GRAPHS);
            writer.WriteStartArray();

            for (int i = 0; i < value.graphs.Count; i++)
            {
                WriteGraph(writer, value.graphs[i]);
            }

            writer.WriteEndArray();
            writer.WritePropertyName(ConstStrings.GRAPH);
            writer.WriteValue(0); // TODO: Default graph selection for users?
            writer.WriteEndObject();
        }

        private void WriteGraph(JsonWriter writer, Graph graph)
        {
            writer.WriteStartObject();
            var typeIndexByType = TypesSerializer.GetSystemTypeByIndexDictionary(graph);
            var declarations = DeclarationsSerializer.GetDeclarations(graph.nodes, typeIndexByType);
            TypesSerializer.WriteJson(writer, graph.types);
            VariablesSerializer.WriteJson(writer, graph.variables, typeIndexByType);
            EventsSerializer.WriteJson(writer, graph.customEvents);
            DeclarationsSerializer.WriteJson(writer, declarations);
            NodesSerializer.WriteJson(writer, graph.nodes, declarations, typeIndexByType);
            writer.WriteEndObject();
        }

        public override KHR_ExtensionGraph ReadJson(JsonReader reader, System.Type objectType, KHR_ExtensionGraph existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);

            var interactivity = new KHR_ExtensionGraph();

            JToken jGraphs = null;
            if (jObj[ConstStrings.GRAPHS] != null)
            {
                jGraphs = jObj[ConstStrings.GRAPHS];

                foreach (JObject jGraph in jGraphs)
                {
                    interactivity.graphs.Add(GenerateGraph(jGraph));
                }

                interactivity.defaultGraphIndex = jObj[ConstStrings.GRAPH].Value<int>();
                interactivity.type = KHR_ExtensionGraph.GraphType.Interactivity;
            }
            else if (jObj[ConstStrings.AUDIO] != null || jObj[ConstStrings.AUDIO_SOURCES] != null || jObj[ConstStrings.AUDIO_EMITTERS] != null)
            {
                var audioGraph = jObj[ConstStrings.AUDIO];
                var audioSourceGraph = jObj[ConstStrings.AUDIO_SOURCES];
                var audioEmitterGraph = jObj[ConstStrings.AUDIO_EMITTERS];

                interactivity.type = KHR_ExtensionGraph.GraphType.Audio;
                var graph = new Graph();

                if (audioGraph != null && audioGraph.HasValues)
                {
                    GenerateAudioGraph(jObj, ref graph);
                }
                if (audioSourceGraph != null && audioSourceGraph.HasValues)
                {
                    GenerateAudioSourcesGraph(jObj, ref graph);
                }
                if (audioEmitterGraph != null && audioEmitterGraph.HasValues)
                {
                    GenerateAudioEmittersGraph(jObj, ref graph);
                }

                interactivity.graphs.Add(graph);
                //foreach (JObject jGraph in audioGraph)
                //{
                //    interactivity.graphs.Add(GenerateAudioGraph(jGraph, ref graph));
                //}
                //foreach (JObject jGraph in audioSourceGraph)
                //{
                //    interactivity.graphs.Add(GenerateAudioSourcesGraph(jGraph, ref graph));
                //}
                //foreach (JObject jGraph in audioEmitterGraph)
                //{
                //    interactivity.graphs.Add(GenerateAudioEmittersGraph(jGraph, ref graph));
                //}

            }
            //switch (existingValue.type)
            //{
            //    case KHR_ExtensionGraph.GraphType.Interactivity:
            //        jGraphs = jObj[ConstStrings.GRAPHS];
            //        break;
            //    case KHR_ExtensionGraph.GraphType.Audio:
            //        jGraphs = jObj[ConstStrings.AUDIO];
            //        break;
            //    default:
            //        jGraphs = jObj[ConstStrings.GRAPHS];
            //        break;
            //}



            return interactivity;
        }

        private static void GenerateAudioGraph(JObject jObj, ref Graph graph)
        {
            var audio = AudioDeserializer.GetAudio(jObj);

            graph.audio = audio;
        }

        private static void GenerateAudioSourcesGraph(JObject jObj, ref Graph graph)
        {
            var sources = AudioSourceDeserializer.GetAudioSources(jObj);

            graph.audioSources = sources; 
        }

        private static void GenerateAudioEmittersGraph(JObject jObj, ref Graph graph)
        {
            var emitters = AudioEmittersDeserializer.GetAudioEmitters(jObj);

            graph.audioEmitter = emitters;
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