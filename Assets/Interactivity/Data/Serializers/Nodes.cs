using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityGLTF.Interactivity
{
    public static class NodesSerializer
    {
        public static void WriteJson(JsonWriter writer, List<Node> nodes, Dictionary<Type, int> typeIndexByType)
        {
            writer.WritePropertyName(ConstStrings.NODES);
            writer.WriteStartArray();

            for (int i = 0; i < nodes.Count; i++)
            {
                WriteNode(writer, nodes, i, typeIndexByType);
            }

            writer.WriteEndArray();
        }

        private static void WriteNode(JsonWriter writer, List<Node> nodes, int nodeIndex, Dictionary<Type, int> typeIndexByType)
        {
            var node = nodes[nodeIndex];

            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.TYPE);
            writer.WriteValue(node.type);

            WriteValues(writer, nodes, nodeIndex, typeIndexByType);
            WriteConfiguration(writer, node.configuration);
            WriteFlows(writer, nodes, nodeIndex);
            WriteMetadata(writer, node.metadata);

            writer.WriteEndObject();
        }

        private static void WriteMetadata(JsonWriter writer, Metadata metadata)
        {
            writer.WritePropertyName(ConstStrings.METADATA);
            writer.WriteStartObject();

            writer.WritePropertyName("positionX");
            writer.WriteValue(metadata.positionX.ToString());

            writer.WritePropertyName("positionY");
            writer.WriteValue(metadata.positionY.ToString());

            writer.WriteEndObject();
        }

        private static void WriteFlows(JsonWriter writer, List<Node> nodes, int nodeIndex)
        {
            var flows = nodes[nodeIndex].flows;

            writer.WritePropertyName(ConstStrings.FLOWS);
            writer.WriteStartArray();

            for (int i = 0; i < flows.Count; i++)
            {
                WriteFlow(writer, nodes, flows[i]);
            }

            writer.WriteEndArray();
        }

        private static void WriteFlow(JsonWriter writer, List<Node> nodes, Flow flow)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.ID);
            writer.WriteValue(flow.fromSocket);

            writer.WritePropertyName(ConstStrings.NODE);

            var targetNodeIndex = nodes.IndexOf(flow.toNode);
            Assert.AreNotEqual(targetNodeIndex, -1);
            writer.WriteValue(targetNodeIndex);

            writer.WritePropertyName(ConstStrings.SOCKET);
            writer.WriteValue(flow.toSocket);

            writer.WriteEndObject();
        }

        private static void WriteConfiguration(JsonWriter writer, List<Configuration> configuration)
        {
            writer.WritePropertyName(ConstStrings.CONFIGURATION);
            writer.WriteStartArray();

            for (int i = 0; i < configuration.Count; i++)
            {
                WriteConfigurationEntry(writer, configuration[i]);
            }

            writer.WriteEndArray();
        }

        private static void WriteConfigurationEntry(JsonWriter writer, Configuration configuration)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.ID);
            writer.WriteValue(configuration.id);

            writer.WritePropertyName(ConstStrings.VALUE);

            if (configuration.value is JArray array)
                array.WriteTo(writer);
            else
                writer.WriteValue(configuration.value);

            writer.WriteEndObject();
        }

        private static void WriteValues(JsonWriter writer, List<Node> nodes, int nodeIndex, Dictionary<Type, int> typeIndexByType)
        {
            var values = nodes[nodeIndex].values;

            writer.WritePropertyName(ConstStrings.VALUES);
            writer.WriteStartArray();

            for (int i = 0; i < values.Count; i++)
            {
                WriteValue(writer, nodes, values[i], typeIndexByType);
            }

            writer.WriteEndArray();
        }

        private static void WriteValue(JsonWriter writer, List<Node> nodes, Value value, Dictionary<Type, int> typeIndexByType)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.ID);
            writer.WriteValue(value.id);

            if (value.node != null)
            {
                writer.WritePropertyName(ConstStrings.NODE);
                var targetNodeIndex = nodes.IndexOf(value.node);
                Assert.AreNotEqual(targetNodeIndex, -1);
                writer.WriteValue(targetNodeIndex);

                writer.WritePropertyName(ConstStrings.SOCKET);
                writer.WriteValue(value.socket);
            }
            else
            {
                WriteValueLiteral(writer, value.property, typeIndexByType);
            }

            writer.WriteEndObject();
        }

        public static void WriteValueLiteral(JsonWriter writer, IProperty property, Dictionary<Type, int> typeIndexByType)
        {
            writer.WritePropertyName(ConstStrings.VALUE);
            writer.WriteStartArray();

            var type = Constants.INVALID_TYPE_INDEX;

            switch (property)
            {
                case Property<int> iProp:
                    writer.WriteValue(iProp.value);
                    type = typeIndexByType[typeof(int)];
                    break;
                case Property<float> fProp:
                    writer.WriteValue(fProp.value);
                    type = typeIndexByType[typeof(float)];
                    break;
                case Property<bool> bProp:
                    writer.WriteValue(bProp.value);
                    type = typeIndexByType[typeof(bool)];
                    break;
                case Property<float2> f2Prop:
                    writer.WriteValue(f2Prop.value.x);
                    writer.WriteValue(f2Prop.value.y);
                    type = typeIndexByType[typeof(float2)];
                    break;
                case Property<float3> f3Prop:
                    writer.WriteValue(f3Prop.value.x);
                    writer.WriteValue(f3Prop.value.y);
                    writer.WriteValue(f3Prop.value.z);
                    type = typeIndexByType[typeof(float3)];
                    break;
                case Property<float4> f4Prop:
                    writer.WriteValue(f4Prop.value.x);
                    writer.WriteValue(f4Prop.value.y);
                    writer.WriteValue(f4Prop.value.z);
                    writer.WriteValue(f4Prop.value.w);
                    type = typeIndexByType[typeof(float4)];
                    break;
                default:
                    throw new NotImplementedException();
            }

            writer.WriteEndArray();

            writer.WritePropertyName(ConstStrings.TYPE);
            writer.WriteValue(type);
        }
    }
}