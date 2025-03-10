using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class EventOnSelect : BehaviourEngineNode
    {
        // TODO: Add this limitation from spec:
        // A behavior graph MUST NOT contain two or more event/onSelect nodes with the same nodeIndex configuration value.

        // Default values grabbed from spec
        private int _selectedNodeIndex = -1;
        private float3 _selectionPoint = new float3(float.NaN, float.NaN, float.NaN);
        private float3 _selectionRayOrigin = new float3(float.NaN, float.NaN, float.NaN);

        private Transform _parentNode = null;

        public EventOnSelect(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onSelect += OnSelect;

            if (!configuration.TryGetValue(ConstStrings.NODE_INDEX, out Configuration config))
                return;

            var parentIndex = Parser.ToInt(config.value);

            _parentNode = engine.pointerResolver.nodePointers[parentIndex].gameObject.transform;
        }

        public override IProperty GetOutputValue(string id)
        {
            return id switch
            {
                ConstStrings.SELECTED_NODE_INDEX =>  new Property<int>(_selectedNodeIndex),
                ConstStrings.SELECTION_POINT =>      new Property<float3>(_selectionPoint),
                ConstStrings.SELECTION_RAY_ORIGIN => new Property<float3>(_selectionRayOrigin),
                _ => throw new InvalidOperationException($"Socket {id} is not valid for this node!"),
            };
        }

        private void OnSelect(Ray ray, RaycastHit hit, RaycastHit[] otherHits)
        {
            // TODO: Add support for stopPropagation once we understand what it actually does.
            // I've read that part of the spec a handful of times and still am not sure.
            var t = hit.transform;
            var go = t.gameObject;
            var nodeIndex = engine.pointerResolver.IndexOf(go);

            var shouldExecute = true;

            // If there's a parent node provided in the config and it's not the node we clicked on
            if (_parentNode != null && _parentNode != t)
            {
                shouldExecute = false;
                // Go up the hierarchy and figure out if the selected node is a child of the nodeIndex provided in the config.
                while (t.parent != null)
                {
                    if (t.parent == _parentNode)
                    {
                        shouldExecute = true;
                        break;
                    }

                    t = t.parent;
                }
            }

            // Node was not a child of the nodeIndex from the config, so we shouldn't execute our flow or set any values.
            if (!shouldExecute)
                return;

            _selectedNodeIndex = nodeIndex;
            _selectionPoint = hit.point;
            _selectionRayOrigin = ray.origin;

            Util.Log($"OnSelect node {nodeIndex} corresponding to GO {go.name}", go);

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}