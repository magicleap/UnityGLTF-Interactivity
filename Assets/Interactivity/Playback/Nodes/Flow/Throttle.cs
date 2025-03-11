using System;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowThrottle : BehaviourEngineNode
    {
        private float _duration;
        private float _lastRemainingTime;

        public FlowThrottle(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onTick += PlayTimeIncreased;
        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            if (_lastRemainingTime >= _duration)
            {
                _lastRemainingTime = 0;
                TryExecuteFlow(ConstStrings.LOOP_BODY);
            }
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.DURATION, out float duration))
                return false;

            _duration = duration;
            return true;
        }

        public override bool ValidateFlows(string socket)
        {
            if (Validate(socket) != ValidationResult.Valid)
            {
                Util.LogError("Input flow is invalid!");
                return false;
            }



            return base.ValidateFlows(socket);
        }

        public override IProperty GetOutputValue(string socket)
        {
            if (TryEvaluateValue(ConstStrings.OUTPUT_VALUE_SOCKETS, out float lastRemainingTime))
            {
                return new Property<float>(lastRemainingTime);
            }
            
            Util.LogError("The output value socket is the wrong type of input!");
            return base.GetOutputValue(socket);
        }

        private void PlayTimeIncreased()
        {
            _lastRemainingTime += Time.deltaTime;
        }
    }
}