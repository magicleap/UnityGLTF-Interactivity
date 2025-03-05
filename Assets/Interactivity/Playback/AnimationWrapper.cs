using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityGLTF.Interactivity
{
    public struct AnimationData
    {
        public int index;
        public float startTime;
        public float endTime;
        public float stopTime;
        public float speed;
        public float unityStartTime;
        public Action endDone;
        public Action stopDone;
    }

    public class AnimationWrapper : MonoBehaviour
    {
        public Animation animationComponent { get; private set; }

        private AnimationState _currentAnimation;
        private AnimationState[] _animations;

        private readonly Dictionary<int, AnimationData> _animationsInProgress = new();

        private BehaviourEngine _engine;

        public void SetData(BehaviourEngine behaviourEngine, Animation animationComponent)
        {
            if (_engine != null)
                _engine.onTick -= OnTick;

            _engine = behaviourEngine;
            _engine.onTick += OnTick;
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

        private void OnTick()
        {
            // Avoiding iterating over a changing collection by grabbing a pooled dictionary.
            var temp = DictionaryPool<int, AnimationData>.Get();
            try
            {
                foreach (var anim in _animationsInProgress)
                {
                    temp.Add(anim.Key, anim.Value);
                }

                foreach (var anim in temp)
                {
                    SampleAnimation(anim.Value);
                }
            }
            finally
            {
                DictionaryPool<int, AnimationData>.Release(temp);
            }
        }

        // This logic path hurts my soul but it's taken directly from the spec.
        // A lot harder to follow than what we had before.
        private bool SampleAnimation(AnimationData a)
        {
            float r, T;

            T = _animations[a.index].length;

            if (a.startTime == a.endTime)
            {
                r = a.startTime;
                CompleteAnimation(r, a.endDone);
                return false;
            }

            var scaledElapsedTime = (Time.time - a.unityStartTime) * a.speed;

            if (a.startTime > a.endTime)
                scaledElapsedTime *= -1;

            r = scaledElapsedTime + a.startTime;

            var c1 = a.startTime < a.endTime && r >= a.stopTime && a.stopTime >= a.startTime && a.stopTime < a.endTime;
            var c2 = a.startTime > a.endTime && r <= a.stopTime && a.stopTime <= a.startTime && a.stopTime > a.endTime;

            if (c1 || c2)
            {
                r = a.stopTime;
                Util.Log($"Stopping Animation {a.index}.");
                CompleteAnimation(r, a.stopDone);
                return false;
            }

            var c3 = a.startTime < a.endTime && r >= a.endTime;
            var c4 = a.startTime > a.endTime && r <= a.endTime;

            if (c3 || c4)
            {
                r = a.endTime;
                Util.Log($"Done Animation {a.index}.");
                CompleteAnimation(r, a.endDone);
                return false;
            }

            SampleAnimationAtTime(r);

            return true;

            float GetTimeStamp(float r)
            {
                var s = r > 0 ? Mathf.Ceil((r - T) / T) : Mathf.Floor(r / T);
                return T == 0 ? 0 : r - s * T;
            }

            void SampleAnimationAtTime(float r)
            {
                _animations[a.index].time = GetTimeStamp(r);
                animationComponent.Sample();
            }

            void CompleteAnimation(float t, Action callback)
            {
                SampleAnimationAtTime(t);
                StopAnimation(a.index);
                callback();
            }
        }

        public void PlayAnimation(in AnimationData data)
        {
            StopAnimation(data.index);

            _animationsInProgress.Add(data.index, data);

            _currentAnimation = _animations[data.index];
            animationComponent.clip = _currentAnimation.clip;
            animationComponent.Play();
        }

        internal void StopAnimationAt(int animationIndex, float stopTime, Action callback)
        {
            var anim = _animationsInProgress[animationIndex];

            anim.stopTime = stopTime;
            anim.stopDone = callback;

            _animationsInProgress[animationIndex] = anim;
        }

        internal void StopAnimation(int index)
        {
            _animationsInProgress.Remove(index);
        }
    }
}