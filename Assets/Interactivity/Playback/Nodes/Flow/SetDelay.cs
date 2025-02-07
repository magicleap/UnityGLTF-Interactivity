using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowSetDelay : BehaviourEngineNode
    {
        public const float MAX_DELAY = 60f;

        private float _duration;
        private bool _cancel;

        public FlowSetDelay(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string socket)
        {
            if (socket == ConstStrings.LAST_DELAY_INDEX)
                throw new NotImplementedException("No idea what this should do so it never got implemented."); // TODO

            throw new ArgumentException($"Socket {socket} is not valid on this node!");
        }

        protected override async void Execute(string socket, ValidationResult validationResult)
        {
            switch (socket)
            {
                case "in":
                    await HandleInSocket(validationResult);
                    break;

                case "cancel":
                    HandleCancelSocket();
                    break;

                default:
                    throw new ArgumentException("Not a valid input socket for this node.");
            }
        }

        private void HandleCancelSocket()
        {
            _cancel = true;
        }

        private async Task HandleInSocket(ValidationResult validationResult)
        {
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            Debug.Log($"Starting a delay of {_duration}s");

            TryExecuteFlow(ConstStrings.OUT);

            var duration = _duration;

            while (duration > 0)
            {
                if (_cancel)
                {
                    _cancel = false;
                    break;
                }

                duration -= Time.deltaTime;
                await Task.Yield();
            }

            TryExecuteFlow(ConstStrings.DONE);
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
    }
}