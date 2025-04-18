using System;
using System.Collections.Generic;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public struct AnimationPointers
    {
        public ReadOnlyPointer<bool> isPlaying;
        public ReadOnlyPointer<float> playhead;
        public ReadOnlyPointer<float> virtualPlayhead;
        public ReadOnlyPointer<float> minTime;
        public ReadOnlyPointer<float> maxTime;

        public AnimationPointers(AnimationWrapper wrapper, int animationIndex)
        {
            isPlaying = new ReadOnlyPointer<bool>(() => wrapper.IsAnimationPlaying(animationIndex));
            playhead = new ReadOnlyPointer<float>(() => wrapper.GetPlayhead(animationIndex));
            virtualPlayhead = new ReadOnlyPointer<float>(() => wrapper.GetVirtualPlayhead(animationIndex));
            minTime = new ReadOnlyPointer<float>(() => 0f);
            maxTime = new ReadOnlyPointer<float>(() => wrapper.GetAnimationMaxTime(animationIndex));
        }

        public static IPointer ProcessPointer(StringSpanReader reader, BehaviourEngineNode engineNode, List<AnimationPointers> pointers)
        {
            reader.AdvanceToNextToken('/');

            var nodeIndex = PointerResolver.GetIndexFromArgument(reader, engineNode);

            var pointer = pointers[nodeIndex];

            reader.AdvanceToNextToken('/');
            reader.AdvanceToNextToken('/');
            reader.AdvanceToNextToken('/');

            // Path so far: /animations/{}/extensions/KHR_interactivity
            return reader.AsReadOnlySpan() switch
            {
                var a when a.Is(Pointers.IS_PLAYING) => pointer.isPlaying,
                var a when a.Is(Pointers.MIN_TIME) => pointer.minTime,
                var a when a.Is(Pointers.MAX_TIME) => pointer.maxTime,
                var a when a.Is(Pointers.PLAYHEAD) => pointer.playhead,
                var a when a.Is(Pointers.VIRTUAL_PLAYHEAD) => pointer.virtualPlayhead,
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }
    }
}