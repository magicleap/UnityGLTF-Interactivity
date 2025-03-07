using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowWhile : BehaviourEngineNode
    {
        bool valid;
        public FlowWhile(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            while (socket != ConstStrings.IN)
                throw new ArgumentException($"Only valid input socket for this node is \"{ConstStrings.IN}\"");

            TryEvaluateValue(ConstStrings.CONDITION, out valid);

            while (valid)
            {
                TryExecuteFlow(ConstStrings.LOOP_BODY);
                TryEvaluateValue(ConstStrings.CONDITION, out valid);
            }

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.CONDITION, out bool condition);
        }
    }
}