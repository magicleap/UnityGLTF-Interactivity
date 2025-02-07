using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class Graph
    {
        public List<Node> nodes { get; set; } = new();
        public List<Variable> variables { get; set; } = new();
        public List<Customevent> customEvents { get; set; } = new();
        public List<InteractivityType> types { get; set; } = new();

        public event Action<Node> onNodeAdded;
        public event Action<Variable> onVariableAdded;
        public event Action<Customevent> onEventAdded;
        public event Action<InteractivityType> onTypeAdded;

        public event Action<Node> onNodeRemoved;

        public Variable AddVariable<T>(string id, T initialValue)
        {
            var variable = new Variable()
            {
                id = id,
                property = new Property<T>(initialValue),
                initialValue = new Property<T>(initialValue),
            };

            variables.Add(variable);

            onVariableAdded?.Invoke(variable);

            return variable;
        }

        public Customevent AddEvent(string id)
        {
            var e = new Customevent()
            {
                id = id
            };

            customEvents.Add(e);

            onEventAdded?.Invoke(e);

            return e;
        }

        public Node CreateNode(string type, Vector2 position)
        {
            var e = new Node()
            {
                type = type
            };

            e.AddDefaultData();
            e.SetPositionMetadata(position.x, position.y);

            nodes.Add(e);

            onNodeAdded?.Invoke(e);

            return e;
        }

        public bool RemoveNode(Node node)
        {
            var success = nodes.Remove(node);

            if (!success)
                return false;

            node.OnRemovedFromGraph();
            onNodeRemoved?.Invoke(node);
            return true;
        }

        public InteractivityType AddType(string signature, TypeExtensions extensions = null)
        {
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].signature == signature)
                {
                    Debug.LogWarning($"Type {signature} has already been added to the list of types.");
                    return types[i];
                }
            }

            var newType = new InteractivityType() { signature = signature, extensions = extensions };
            types.Add(newType);
            onTypeAdded?.Invoke(newType);

            return newType;
        }

        public void AddDefaultTypes()
        {
            types = new List<InteractivityType>()
            {
                new InteractivityType() { signature = "bool" },
                new InteractivityType() { signature = "int" },
                new InteractivityType() { signature = "float" },
                new InteractivityType() { signature = "float2" },
                new InteractivityType() { signature = "float3" },
                new InteractivityType() { signature = "float4" },
                new InteractivityType() { signature = "float4x4" },
                new InteractivityType() { signature = "custom", extensions = new TypeExtensions() { AMZN_interactivity_string = new() } },
            };
        }
    }
}