using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowThrottle : BehaviourEngineNode
    {
        private float _duration;
        private float _timestamp;
        private float _elapsed;
        private float _lastRemainingTime = float.NaN;

        public FlowThrottle(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (socket.Equals(ConstStrings.RESET))
            {
                _lastRemainingTime = float.NaN;
                return;
            }
                 
            TryEvaluateValue(ConstStrings.DURATION, out _duration);
            if (_duration >= 0 && !float.IsNaN(_duration) && !float.IsInfinity(_duration))
            {
                if (!float.IsNaN(_lastRemainingTime))
                {
                    _elapsed = Time.time - _timestamp;
                    if (_duration > _elapsed)
                        _lastRemainingTime = _duration - _elapsed;
                    else
                        ExecuteOutFlow();
                }
                else
                    ExecuteOutFlow();
            }
            else
                TryExecuteFlow(ConstStrings.ERR);    
        }

        private void ExecuteOutFlow()
        {
            _timestamp = Time.time;
            _lastRemainingTime = 0;
            TryExecuteFlow(ConstStrings.OUT);
        }

        public override IProperty GetOutputValue(string socket)
        {
            return new Property<float>(_lastRemainingTime);
        }
    }
}