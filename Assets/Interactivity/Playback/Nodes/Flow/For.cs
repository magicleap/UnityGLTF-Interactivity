using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowFor : BehaviourEngineNode
    {
        private int _startIndex;
        private int _endIndex;
        private int _index;

        public FlowFor(BehaviourEngine engine, Node node) : base(engine, node)
        {
            _index = Parser.ToInt(configuration[ConstStrings.INITIAL_INDEX].value);
        }

        public override IProperty GetOutputValue(string socket)
        {
            if (socket == ConstStrings.INDEX)
                return new Property<int>(_index);

            throw new ArgumentException($"Socket {socket} is not valid on this node!");
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            Util.Log($"Starting a loop with start index {_startIndex} and end index {_endIndex} from initial value {_index}");

            for (int i = _index < _startIndex ? _startIndex : _index; i < _endIndex; i++)
            {
                _index = i;
                TryExecuteFlow(ConstStrings.LOOP_BODY);
                Util.Log($"Loop is on iteration {i} and the index reads as {_index}");
            }

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.START_INDEX, out _startIndex) ||
            !TryEvaluateValue(ConstStrings.END_INDEX, out _endIndex))
                return false;

            return true;
        }
    }
}