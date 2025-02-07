using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace UnityGLTF.Interactivity
{
    public static class NodesDeserializer
    {
        private struct NodePair
        {
            public Node node;
            public JToken jToken;
        }

        public static List<Node> GetNodes(JObject jObj, List<Type> types)
        {
            var jNodes = jObj[ConstStrings.NODES].Children();

            var nodeCount = jNodes.Count();

            var nodes = new List<Node>(nodeCount);

            var nodePairs = ListPool<NodePair>.Get();

            Node node;

            try
            {
                foreach (var jToken in jNodes)
                {
                    node = new Node()
                    {
                        type = jToken[ConstStrings.TYPE].Value<string>(),
                        metadata = GetMetadata(jToken[ConstStrings.METADATA]),
                        configuration = GetConfiguration(jToken[ConstStrings.CONFIGURATION])
                    };

                    nodes.Add(node);
                    nodePairs.Add(new NodePair()
                    {
                        node = node,
                        jToken = jToken
                    });
                }

                foreach (var nodePair in nodePairs)
                {
                    nodePair.node.values = GetValues(nodePair.jToken[ConstStrings.VALUES], nodes, types);
                    nodePair.node.flows = GetFlows(nodePair.node, nodePair.jToken[ConstStrings.FLOWS], nodes);
                }
            }
            finally
            {
                ListPool<NodePair>.Release(nodePairs);
            }

            return nodes;
        }

        private static List<Flow> GetFlows(Node fromNode, JToken jToken, List<Node> nodes)
        {
            var count = jToken.Count();
            var flows = new List<Flow>(count);

            foreach (var v in jToken)
            {

                flows.Add(new Flow(
                    fromNode,
                    v[ConstStrings.ID].Value<string>(),
                    nodes[v[ConstStrings.NODE].Value<int>()],
                    v[ConstStrings.SOCKET].Value<string>()));
            }

            return flows;
        }

        private static List<Value> GetValues(JToken jToken, List<Node> nodes, List<Type> types)
        {
            var count = jToken.Count();
            var values = new List<Value>(count);

            foreach (var v in jToken)
            {
                Debug.Log(v);
                var jType = v[ConstStrings.TYPE];
                var jNode = v[ConstStrings.NODE];
                var jSocket = v[ConstStrings.SOCKET];
                var jValue = v[ConstStrings.VALUE];

                int type = Constants.INVALID_TYPE_INDEX;
                Node node = null;
                var socket = Constants.EMPTY_SOCKET_STRING;
                IProperty value = null;

                if (jType != null)
                {
                    type = jType.Value<int>();

                    Assert.IsNotNull(jValue);

                    value = Helpers.CreateProperty(types[type], jValue);
                }

                if (jNode != null)
                    node = nodes[jNode.Value<int>()];

                if (jSocket != null)
                    socket = jSocket.Value<string>();

                var id = v[ConstStrings.ID].Value<string>();

                values.Add(new Value()
                {
                    id = id,
                    property = value,
                    node = node,
                    socket = socket
                });

                Debug.Log($"Created property {id} connected to node {node} and socket {socket} with type {type}");
            }

            return values;
        }

        private static List<Configuration> GetConfiguration(JToken jToken)
        {
            var count = jToken.Count();
            var configuration = new List<Configuration>(count);

            foreach (var v in jToken)
            {
                configuration.Add(new Configuration()
                {
                    id = v[ConstStrings.ID].Value<string>(),
                    value = v[ConstStrings.VALUE].ToObject<object>()
                });
            }

            return configuration;
        }

        private static Metadata GetMetadata(JToken jToken)
        {
            return new Metadata()
            {
                positionX = double.Parse(jToken["positionX"].Value<string>()),
                positionY = double.Parse(jToken["positionY"].Value<string>()),
            };
        }
    }
}