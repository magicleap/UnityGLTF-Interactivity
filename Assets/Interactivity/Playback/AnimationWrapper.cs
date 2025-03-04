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

        public async Task<bool> PlayAnimationAsync<T>(int animationIndex, float startTime, float endTime, float speed, T cancellationToken) where T : struct, ICancelToken
        {
            if (animationComponent == null)
                throw new InvalidOperationException("No animations present in this glb!");

            if (_currentAnimation != null)
                SampleAnimationAtTime(0);

            _currentAnimation = _animations[animationIndex];
            animationComponent.clip = _currentAnimation.clip;
            animationComponent.Play();

            var clipTime = _currentAnimation.clip.length;

            for (float t = startTime; t < endTime; t += Time.deltaTime / clipTime * speed)
            {
                if (cancellationToken.isCancelled)
                    return false;

                SampleAnimationAtTime(t * clipTime);
                await Task.Yield();
            }

            SampleAnimationAtTime(endTime * clipTime);

            return true;
        }
    }
}