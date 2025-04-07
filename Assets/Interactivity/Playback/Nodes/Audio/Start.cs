using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AudioStart : BehaviourEngineNode
    {
        private int _audioSourceIdx;
        public AudioStart(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            Util.Log($"Starting audio index {_audioSourceIdx} ");

            TryExecuteFlow(ConstStrings.OUT);

            var data = new AudioPlayData()
            {
                pauseTime = 0,
                stopTime = 0,
                index = _audioSourceIdx,
                state = AudioWrapper.AudioState.Stopped,
                actionDone = () => TryExecuteFlow(ConstStrings.DONE)
            };

            engine.PlayAudio(data);

            //engine.PlayAnimation(data);
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.AUDIO_SOURCE_INDEX, out _audioSourceIdx);
        }

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