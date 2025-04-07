using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AudioStop : BehaviourEngineNode
    {
        private int _audioSourceIdx;

        public AudioStop(BehaviourEngine engine, Node node) : base(engine, node)
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

            engine.StopAudio(data);
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.AUDIO_SOURCE_INDEX, out _audioSourceIdx);
        }
    }
}