using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowThrottle : BehaviourEngineNode
    {
        private float _duration;
        private float _timestamp = -1f;

        public FlowThrottle(BehaviourEngine engine, Node node) : base(engine, node)
        {
            _timestamp = Time.time;
        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            Util.Log($"Starting a loop at {_timestamp}");

            if (_timestamp < 0 || _timestamp + _duration <= Time.time)
                TryExecuteFlow(ConstStrings.LOOP_BODY);

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