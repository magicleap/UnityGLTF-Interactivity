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

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            Util.Log($"Starting a loop with start index {_startIndex} and end index {_endIndex}");

            for (_index = _startIndex; _index < _endIndex; _index++)
            {
                TryExecuteFlow(ConstStrings.LOOP_BODY);
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