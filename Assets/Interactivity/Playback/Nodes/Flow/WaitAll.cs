using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace UnityGLTF.Interactivity
{
    public class FlowWaitAll : BehaviourEngineNode
    {
        private List<Flow> inputFlows;
        private HashSet<Node> _remainingNodes;
        private Property<int> remainingCount = new Property<int>();
        private bool _reset;

        public FlowWaitAll(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onFlowTriggered += CheckInputFlows;
            if (!TryGetConfig(ConstStrings.INPUT_FLOWS, out inputFlows))
                return;
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            Util.Log("Waiting for all");

            if (inputFlows.Count <= 0) return;

            if (_remainingNodes.Count <= 0)
                TryExecuteFlow(ConstStrings.COMPLETED);
            else
                TryExecuteFlow(ConstStrings.OUT);
        }

        public override IProperty GetOutputValue(string socket)
        {
            return remainingCount;
        }

        private void CheckInputFlows(Flow flow)
        {
            if (_remainingNodes.Contains(flow.fromNode))
                _remainingNodes.Remove(flow.fromNode);

            
        }
    }
}