using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class EventOnSelect : BehaviourEngineNode
    {
        private bool _selectionPerformed;
        private int _nodeIndex;

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
            if (!_selectionPerformed)
                throw new InvalidOperationException("Nothing has been selected but there is a request for an output value of the select node!");

            switch (id)
            {
                case ConstStrings.SELECTED_NODE_INDEX:
                    return new Property<int>(_nodeIndex);
            }

            throw new InvalidOperationException($"Socket {id} is not valid for this node!");
        }

        private void OnSelect(RaycastHit hit, RaycastHit[] otherHits)
        {
            var t = hit.transform;
            var go = t.gameObject;
            var nodeIndex = engine.pointerResolver.IndexOf(go);

            var shouldExecute = true;

            if (_parentNode != null)
            {
                shouldExecute = false;
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

            if (!shouldExecute)
                return;

            _nodeIndex = nodeIndex;
            _selectionPerformed = true;

            Util.Log($"OnSelect node {nodeIndex} corresponding to GO {go.name}", go);

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}