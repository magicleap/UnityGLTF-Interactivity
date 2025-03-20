using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            Node nodeToCancel = engine.graph.nodes[delayIndex];
            FlowSetDelay x = (FlowSetDelay)engine.engineNodes[node];
            if (nodeToCancel == null || x == null)
                return;

            //x.SetCancel(true);

            TryExecuteFlow(ConstStrings.COMPLETED);
        }
    }
}