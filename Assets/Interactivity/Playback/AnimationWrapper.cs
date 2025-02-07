using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AnimationWrapper : MonoBehaviour
    {
        public Animation animationComponent { get; private set; }

        private AnimationState _currentAnimation;
        private AnimationState[] _animations;

        private bool _isPlaying;

        public void SetData(Animation animationComponent)
        {
            this.animationComponent = animationComponent;

            var clipCount = animationComponent.GetClipCount();
            _animations = new AnimationState[clipCount];

            var j = 0;

            foreach (AnimationState state in animationComponent)
            {
                state.speed = 0f;
                _animations[j++] = state;
            }
        }

        private void SampleAnimationAtTime(float t)
        {
            _currentAnimation.time = t;
            animationComponent.Sample();
        }

        public async Task PlayAnimationAsync(int animationIndex, float startTime, float endTime, float speed)
        {
            if (animationComponent == null)
                throw new InvalidOperationException("No animations present in this glb!");

            if (_isPlaying) // TODO: Figure out how Khronos intends for us to handle this edge case.
                throw new InvalidOperationException("Tried to play an animation while another is already playing!");

            _isPlaying = true;

            if (_currentAnimation != null)
                SampleAnimationAtTime(0);

            _currentAnimation = _animations[animationIndex];
            animationComponent.clip = _currentAnimation.clip;
            animationComponent.Play();

            var clipTime = _currentAnimation.clip.length;

            for (float t = startTime; t < endTime; t += Time.deltaTime / clipTime * speed)
            {
                SampleAnimationAtTime(t * clipTime);
                await Task.Yield();
            }

            SampleAnimationAtTime(endTime * clipTime);

            _isPlaying = false;
        }
    }
}