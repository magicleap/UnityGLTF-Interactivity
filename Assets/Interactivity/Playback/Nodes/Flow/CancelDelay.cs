using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowCancelDelay : BehaviourEngineNode
    {
        private int delayIndex = 0;
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
            Util.Log($"Canceling the delayed output for the replacement behavior");
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            if (TryEvaluateValue(ConstStrings.IN, out BehaviourEngineNode inputNode))
            {
                engine.graph.nodes.Remove(inputNode.node);
            }
            else
                engine.graph.nodes.RemoveAt(delayIndex);

            Util.Log($"Cancelling this input node {delayIndex}");
            cancellationToken = new CancellationToken(true);

            if (cancellationToken.IsCancellationRequested)
                return;

            TryExecuteFlow(ConstStrings.COMPLETED);
        }
    }
}