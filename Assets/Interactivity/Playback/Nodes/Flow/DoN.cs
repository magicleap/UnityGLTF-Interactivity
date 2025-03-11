using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowDoN : BehaviourEngineNode
    {

        private int _endIndex;
        private int _currentIndex;

        public FlowDoN(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            Util.Log($"Starting a loop with doN {_endIndex}");

            if (TryEvaluateValue(ConstStrings.CONDITION, out bool condition) && condition)
                _currentIndex = 0;

            for (_currentIndex = 0; _currentIndex < _endIndex; _currentIndex++)
            {
                TryExecuteFlow(ConstStrings.LOOP_BODY);
            }

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override IProperty GetOutputValue(string socket)
        {
            bool validOutputValue = TryEvaluateValue(ConstStrings.OUTPUT_VALUE_SOCKETS, out IProperty currentCount);
            return validOutputValue ? currentCount : throw new InvalidOperationException("No supported type found or input types did not match.");
        }
    }
}