using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AudioPause : BehaviourEngineNode
    {
        private int _audioSourceIdx;
        public AudioPause(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            Util.Log($"Playing animation index {_audioSourceIdx}");

            TryExecuteFlow(ConstStrings.OUT);

            var data = new AudioPlayData()
            {
                index = _audioSourceIdx,
                //                endDone = () => TryExecuteFlow(ConstStrings.DONE)
            };

            engine.PauseAudio(data);
        }


        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.AUDIO_SOURCE_INDEX, out _audioSourceIdx);
        }

        //public override bool ValidateValues(string socket)
        //{
        //    return TryEvaluateValue(ConstStrings.ANIMATION, out _animationIndex) &&
        //        TryEvaluateValue(ConstStrings.SPEED, out _speed) &&
        //        TryEvaluateValue(ConstStrings.START_TIME, out _startTime) &&
        //        TryEvaluateValue(ConstStrings.END_TIME, out _endTime) &&
        //        ValidateAnimationIndex(_animationIndex) &&
        //        ValidateStartAndEndTimes(_startTime, _endTime) &&
        //        ValidateSpeed(_speed);
        //}

        //private static bool ValidateSpeed(float speed)
        //{
        //    return speed > 0 && speed != float.NaN && speed != float.PositiveInfinity;
        //}

        //private bool ValidateAnimationIndex(int animationIndex)
        //{
        //    if (!TryGetReadOnlyPointer($"/{Pointers.ANIMATIONS_LENGTH}", out ReadOnlyPointer<int> animPointer))
        //        return false;

        //    var animationCount = animPointer.GetValue();

        //    if (animationIndex < 0 || animationIndex >= animationCount)
        //        return false;

        //    return true;
        //}

        //private static bool ValidateStartAndEndTimes(float startTime, float endTime)
        //{
        //    if (startTime == float.NaN || endTime == float.NaN)
        //        return false;

        //    if (startTime == float.PositiveInfinity || startTime < 0)
        //        return false;

        //    if (startTime > endTime)
        //        return false;

        //    return true;
        //}
    }
}