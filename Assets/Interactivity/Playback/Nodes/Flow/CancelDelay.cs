using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowCancelDelay : BehaviourEngineNode
    {
        public FlowCancelDelay(BehaviourEngine engine, Node node) : base(engine, node)
        {

        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            Util.Log($"Canceling a delay");

            // for (_index = _startIndex; _index < _endIndex; _index++)
            // {
            //     TryExecuteFlow(ConstStrings.LOOP_BODY);
            // }

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                return false;

            return true;
        }
    }
}