using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowWhile : BehaviourEngineNode
    {
        private bool _condition;

        public FlowWhile(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string socket)
        {
            if (socket == ConstStrings.FALSE)
                return new Property<bool>(_condition);

            throw new ArgumentException($"Socket {socket} is not valid on this node!");
        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            while(socket != ConstStrings.IN)
                throw new ArgumentException($"Only valid input socket for this node is \"{ConstStrings.IN}\"");

            Util.Log($"Branch condition is {_condition}");

            var outSocket = _condition ? ConstStrings.TRUE : ConstStrings.FALSE;

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                return false;

            _condition = condition;
            return true;
        }
    }
}