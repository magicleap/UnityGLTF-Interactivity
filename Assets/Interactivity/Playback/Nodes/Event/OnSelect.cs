using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class EventOnSelect : BehaviourEngineNode
    {
        private bool _selectionPerformed;
        private int _nodeIndex;

        public EventOnSelect(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onSelect += OnSelect;
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

        private void OnSelect(int nodeIndex)
        {
            _nodeIndex = nodeIndex;
            _selectionPerformed = true;

            Debug.Log($"OnSelect");

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}