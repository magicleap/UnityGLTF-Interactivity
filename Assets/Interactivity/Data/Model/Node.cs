using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class Node
    {
        public string type { get; internal set; }
        public List<Value> values { get; internal set; } = new();
        public List<Configuration> configuration { get; internal set; } = new();
        public List<Flow> flows { get; internal set; } = new();
        public Metadata metadata { get; internal set; } = new();

        public event Action<Flow> onFlowAdded;
        public event Action<Flow> onFlowRemoved;

        public event Action onRemovedFromGraph;

        public Value AddValue<T>(string id, T value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (!values[i].id.Equals(id))
                    continue;

                values[i].property = new Property<T>(value);
                return values[i];
            }

            var v = new Value()
            {
                id = id,
                property = new Property<T>(value)
            };

            values.Add(v);

            Util.Log($"Added value {id} with payload {value.ToString()}");

            return v;
        }

        public bool TryGetValueById(string id, out Value value)
        {
            value = null;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].id == id)
                {
                    value = values[i];
                    return true;
                }
            }

            return false;
        }

        public bool TryChangeValueToReference(string id, Node node, string socket, out Value value)
        {
            value = null;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].id == id)
                {
                    value = values[i];

                    if (!value.TryConnectToSocket(node, socket))
                        return false;

                    Util.Log($"Changed value {id} on {type} to reference the output of {node.type}'s value {socket}");
                    return true;
                }
            }

            return false;
        }

        public Flow AddFlow(string id, Node node, string socket)
        {
            for (int i = 0; i < flows.Count; i++)
            {
                if (flows[i].toNode == node && flows[i].fromSocket == id && flows[i].toSocket == socket)
                    throw new InvalidOperationException($"{type} already has a flow from {id} to {socket} on that node {node.type}!");
            }

            var flow = new Flow(this, id, node, socket);

            flows.Add(flow);

            Util.Log($"Created flow between {type} and {node.type} from {id} to {socket}");

            onFlowAdded?.Invoke(flow);

            return flow;
        }

        public bool RemoveFlow(string id)
        {
            Flow toRemove = null;

            for (int i = 0; i < flows.Count; i++)
            {
                if (flows[i].fromSocket == id)
                {
                    toRemove = flows[i];
                    break;
                }
            }

            if (toRemove == null)
                return false;

            return RemoveFlow(toRemove);
        }

        public bool RemoveFlow(Flow flow)
        {
            onFlowRemoved?.Invoke(flow);

            return flows.Remove(flow);
        }

        public Configuration AddConfiguration(string id, JArray value)
        {
            for (int i = 0; i < configuration.Count; i++)
            {
                if (!configuration[i].id.Equals(id))
                    continue;

                configuration[i].value = value;
                return configuration[i];
            }

            var config = new Configuration()
            {
                id = id,
                value = value,
            };

            configuration.Add(config);

            return config;
        }

        public void SetPositionMetadata(double x, double y)
        {
            metadata.positionX = x;
            metadata.positionY = y;
        }

        internal void AddDefaultData()
        {
            var specs = NodeRegistry.nodeSpecs[type];

            var inputs = specs.GetInputs();

            if (inputs.values != null)
            {
                for (int i = 0; i < inputs.values.Length; i++)
                {
                    AddDefaultValueByType(inputs.values[i]);
                }
            }

            // The node data doesn't actually have to know what the output values are.
            // The output info is important for validation/visual graph but not the backing data.
            //var outputs = specs.GetOutputs();

            //for (int i = 0; i < outputs.values.Length; i++)
            //{
            //    AddDefaultValueByType(e, outputs.values[i]);
            //}

            var config = specs.GetConfiguration();

            if (config != null)
            {
                for (int i = 0; i < config.Length; i++)
                {
                    AddConfiguration(config[i].id, new JArray());
                }
            }
        }

        private void AddDefaultValueByType(NodeValue valueData)
        {
            var type = valueData.types[0];

            if (type == typeof(int))
                AddValue(valueData.id, 0);
            else if (type == typeof(float))
                AddValue(valueData.id, 0f);
            else if (type == typeof(bool))
                AddValue(valueData.id, false);
            else if (type == typeof(float2))
                AddValue(valueData.id, float2.zero);
            else if (type == typeof(float3))
                AddValue(valueData.id, float3.zero);
            else if (type == typeof(float4))
                AddValue(valueData.id, float4.zero);
            else if (type == typeof(float2x2))
                AddValue(valueData.id, float2x2.zero);
            else if (type == typeof(float3x3))
                AddValue(valueData.id, float3x3.zero);
            else if (type == typeof(float4x4))
                AddValue(valueData.id, float4x4.zero);
            else
                throw new InvalidOperationException($"No default value available for {type}");
        }

        internal void OnRemovedFromGraph()
        {
            onRemovedFromGraph?.Invoke();
        }
    }
}