using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace UnityGLTF.Interactivity
{
    public class FlowWaitAll : BehaviourEngineNode
    {
        private HashSet<Node> completedNodes;
        public FlowWaitAll(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            Util.Log("Waiting for all");

            if (Parser.ToInt(this.configuration[ConstStrings.INPUT_VALUE_SOCKETS].value) <= 0)
                return;

            if (!TryEvaluateValue(ConstStrings.INPUT_VALUE_SOCKETS, out List<Node> inputNodes))
                return;

            do
                {
                    //TODO refactor
                    foreach (Node node in inputNodes)
                    {
                        if (node.TryGetValueById(ConstStrings.COMPLETED, out Value completed))
                        {
                            if (!completedNodes.TryGetValue(node, out Node isNodeCompleted) && completed.GetValue<bool>())
                            {
                                completedNodes.Add(node);
                            }
                        }
                    }
                } while (completedNodes.Count < inputNodes.Count);
            
            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateFlows(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                return false;

            return true;
        }
    }
}