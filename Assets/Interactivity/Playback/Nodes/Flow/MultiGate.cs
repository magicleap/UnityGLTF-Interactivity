using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowMultiGate : BehaviourEngineNode
    {
        private bool isRandom;
        private bool isLoop;

        public FlowMultiGate(BehaviourEngine engine, Node node) : base(engine, node)
        {

        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            //if is random, execute the random flow
            //otherwise operate like a sequence?
            if (isRandom)
            {
                System.Random rand = new System.Random();
                //GetItems is bugged?
                //int[] randomizedSequence = rand.GetItems<int>(node.flows, node.flows.Count);

                // foreach (int randomOutput in randomizedSequence)
                // {
                //     engine.ExecuteFlow(node.flows[randomOutput]);
                // }
            }
            else
            {
                for (int i = 0; i < node.flows.Count; i++)
                {
                    engine.ExecuteFlow(node.flows[i]);
                }
            }

        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                return false;

            return true;
        }
    }
}