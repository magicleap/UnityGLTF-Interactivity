using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AnimationStart : BehaviourEngineNode
    {
        private int _animationIndex;
        private float _speed;
        private float _startTime;
        private float _endTime;

        private CancellationTokenSource _localCancellationToken = new();

        public AnimationStart(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override async void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            Util.Log($"Playing animation index {_animationIndex} with speed {_speed} and start/end times of {_startTime}/{_endTime}");

            TryExecuteFlow(ConstStrings.OUT);

            CancelAnimationIfApplicable();

            await engine.PlayAnimationAsync(_animationIndex, _startTime, _endTime, _speed, new NodeEngineCancelToken(cancellationToken, _localCancellationToken.Token));

            TryExecuteFlow(ConstStrings.DONE);
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.ANIMATION, out _animationIndex) &&
                TryEvaluateValue(ConstStrings.SPEED, out _speed) &&
                TryEvaluateValue(ConstStrings.START_TIME, out _startTime) &&
                TryEvaluateValue(ConstStrings.END_TIME, out _endTime) &&
                ValidateAnimationIndex(_animationIndex) &&
                ValidateStartAndEndTimes(_startTime, _endTime) &&
                ValidateSpeed(_speed);
        }

        private static bool ValidateSpeed(float speed)
        {
            return speed > 0 && speed != float.NaN && speed != float.PositiveInfinity;
        }

        private bool ValidateAnimationIndex(int animationIndex)
        {
            if (!TryGetReadOnlyPointer($"/{Pointers.ANIMATIONS_LENGTH}", out ReadOnlyPointer<int> animPointer))
                return false;

            var animationCount = animPointer.GetValue();

            if (animationIndex < 0 || animationIndex >= animationCount)
                return false;

            return true;
        }

        private static bool ValidateStartAndEndTimes(float startTime, float endTime)
        {
            if (startTime == float.NaN || endTime == float.NaN)
                return false;

            if (startTime == float.PositiveInfinity || startTime < 0)
                return false;

            if (startTime > endTime)
                return false;

            return true;
        }

        private void CancelAnimationIfApplicable()
        {
            _localCancellationToken.Cancel();
            _localCancellationToken.Dispose();
            _localCancellationToken = new();
        }
    }
}