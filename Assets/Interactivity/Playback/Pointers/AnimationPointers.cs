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
    }
}