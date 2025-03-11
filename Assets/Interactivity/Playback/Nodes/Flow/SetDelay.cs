using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowSetDelay : BehaviourEngineNode
    {
        public const float MAX_DELAY = 60f;
        public bool _cancel;

        private float _duration;
        private float _timeStamp;
        private int indexID;

        public FlowSetDelay(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string socket)
        {
            if (socket == ConstStrings.LAST_DELAY_INDEX)
                throw new NotImplementedException("No idea what this should do so it never got implemented."); // TODO

            throw new ArgumentException($"Socket {socket} is not valid on this node!");
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            Util.Log($"Starting a delay of {_duration}s");
            engine.onTick += ExecuteSetDelay;
        }

        private void ExecuteSetDelay()
        {
            if (_cancel)
                TryExecuteFlow(ConstStrings.DONE);

            if (Time.timeSinceLevelLoad > _timeStamp + _duration)
                TryExecuteFlow(ConstStrings.OUT);
        }
        
        public override bool ValidateValues(string socket)
        {
            if (socket == ConstStrings.CANCEL)
                return true;

            return TryGetDuration(out _duration);
        }

        private bool TryGetDuration(out float duration)
        {
            if (!TryEvaluateValue(ConstStrings.DURATION, out duration))
                return false;

            if (duration == float.NaN || duration == float.PositiveInfinity || duration == float.NegativeInfinity)
                return false;

            if (duration > MAX_DELAY)
                return false;

            return true;
        }

        public void SetCancel(bool cancel)
        {
            _cancel = cancel;
        }
    }
}