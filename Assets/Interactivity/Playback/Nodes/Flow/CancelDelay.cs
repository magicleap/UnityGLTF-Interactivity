using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowCancelDelay : BehaviourEngineNode
    {
        private int delayIndex;
        public FlowCancelDelay(BehaviourEngine engine, Node node) : base(engine, node)
        {

        }

        public override IProperty GetOutputValue(string socket)
        {
            if (socket == ConstStrings.LAST_DELAY_INDEX)
                throw new NotImplementedException("No idea what this should do so it never got implemented."); // TODO

            throw new ArgumentException($"Socket {socket} is not valid on this node!");
        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            Util.Log($"Canceling the delayed output flow");

            TryExecuteFlow(ConstStrings.OUT);
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                return false;

            return true;
        }
    }
}