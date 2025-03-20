using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class Graph
    {
        public List<Type> systemTypes { get; set; } = new();
        public List<Node> nodes { get; set; } = new();
        public List<Variable> variables { get; set; } = new();
        public List<Customevent> customEvents { get; set; } = new();
        public List<InteractivityType> types { get; set; } = new();
        public List<Declaration> declarations { get; set; } = new();

        public event Action<Node> onNodeAdded;
        public event Action<Variable> onVariableAdded;
        public event Action<Customevent> onEventAdded;
        public event Action<InteractivityType> onTypeAdded;

        public event Action<Node> onNodeRemoved;
        public event Action<Variable> onVariableRemoved;

        public Variable AddVariable<T>(string id, T initialValue)
        {
            // Due to how this is overloaded the IProperty cast is required.
            return AddVariable(id, (IProperty)new Property<T>(initialValue));
        }

        public Variable AddVariable(string id, IProperty initialValue)
        {
            var variable = new Variable()
            {
                id = id,
                property = initialValue,
                initialValue = initialValue,
            };

            variables.Add(variable);

            onVariableAdded?.Invoke(variable);

            return variable;
        }

        public bool RemoveVariable(Variable variable)
        {
            var success = variables.Remove(variable);

            if (!success)
                return false;

            onVariableRemoved?.Invoke(variable);
            return true;
        }

        public Customevent AddEvent(string id, List<EventValue> eventValues = null)
        {
            var e = new Customevent()
            {
                id = id,
                values = eventValues
            };

            customEvents.Add(e);

            onEventAdded?.Invoke(e);

            return e;
        }

        public Node CreateNode(string type)
        {
            return CreateNode(type, Vector2.zero);
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
                    Util.LogWarning($"Type {signature} has already been added to the list of types.");
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

        public int IndexOfType(string signature)
        {
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].signature.Equals(signature))
                    return i;
            }

            return -1;
        }

        public bool TryGetVariable(string id, out Variable variable)
        {
            variable = null;
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].id != id)
                    continue;

                variable = variables[i];
                return true;
            }

            return false;
        }

        public IProperty GetDefaultPropertyForType(int typeIndex)
        {
            return Helpers.GetDefaultProperty(typeIndex, systemTypes);
        }
    }
}