using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowThrottle : BehaviourEngineNode
    {
        private float _duration;
        private float _timestamp;
        private float _elapsed;
        private float _lastRemainingTime;

        public FlowThrottle(BehaviourEngine engine, Node node) : base(engine, node)
        {
            _timestamp = Time.time;
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {

            _elapsed = Time.time - _timestamp;
            _lastRemainingTime = _duration - _elapsed;
            switch (socket)
            {
                case ConstStrings.RESET:
                    _lastRemainingTime = float.NaN;
                    break;
                case ConstStrings.IN:
                    if (!CheckValidAndPosFloat(_lastRemainingTime))
                        TryExecuteFlow(ConstStrings.ERR);
                    else
                    {
                        if (float.IsNaN(_lastRemainingTime) || _duration <= _elapsed)
                        {
                            _timestamp = Time.time;
                            _lastRemainingTime = 0;
                            TryExecuteFlow(ConstStrings.OUT);
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Socket {socket} is not a valid input on this Throttle node!");
            }
        }

        private bool CheckValidAndPosFloat(float num)
        {
            if(num < 0 || float.IsNaN(num) || float.IsInfinity(num))
                return false;
            return true;
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.DURATION, out float duration))
                return false;

            _duration = duration;
            return true;
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
    }
}