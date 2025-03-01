using System;
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
        
        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            Util.Log($"Starting a loop with doN {_endIndex}");

            for (_currentIndex = 0; _currentIndex < _endIndex; _currentIndex++)
            {
                TryExecuteFlow(ConstStrings.LOOP_BODY);
            }

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateValues(string socket)
        {
            //set _currentIndex to 0 if this condition happens
            if (TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                _currentIndex = 0;

            if (!TryEvaluateValue(ConstStrings.START_INDEX, out bool _index))
                    return false;

            return true;
        }
    }
}