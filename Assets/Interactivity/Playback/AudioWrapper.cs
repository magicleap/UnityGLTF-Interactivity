using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityGLTF.Interactivity
{
    public struct AudioPlayData
    {
        public int index;
        public string clip;
        public UnityEngine.AudioSource source;
        public float startTime;
        public float endTime;
        public float stopTime;
        public float speed;
    }

    //public class AnimationData
    //{
    //    public float playhead;
    //    public float virtualPlayhead;
    //    public AnimationState anim;

    //    public AnimationData(AnimationState anim)
    //    {
    //        this.anim = anim;
    //    }
    //}

    public class AudioWrapper : MonoBehaviour
    {
        private AudioPlayData _currentAudioData;

        private readonly Dictionary<int, AudioPlayData> _audioSources = new();
        private AnimationData[] _animations;

        private BehaviourEngine _engine;

        public void AddAudioSource(UnityEngine.AudioSource source)
        {
        }

        public void SetData(BehaviourEngine behaviourEngine, AudioPlayData data)
        {
            //if (_engine != null)
            //    _engine.onTick -= OnTick;

            _engine = behaviourEngine;
            //_engine.onTick += OnTick;
            _currentAudioData = data;
        }

        //private void OnTick()
        //{
        //    // Avoiding iterating over a changing collection by grabbing a pooled dictionary.
        //    var temp = DictionaryPool<int, AnimationPlayData>.Get();
        //    try
        //    {
        //        foreach (var anim in _animationsInProgress)
        //        {
        //            temp.Add(anim.Key, anim.Value);
        //        }

        //        foreach (var anim in temp)
        //        {
        //            SampleAnimation(anim.Value);
        //        }
        //    }
        //    finally
        //    {
        //        DictionaryPool<int, AnimationPlayData>.Release(temp);
        //    }
        //}

        //// This logic path hurts my soul but it's taken directly from the spec.
        //// A lot harder to follow than what we had before.
        //private bool SampleAnimation(AnimationPlayData a)
        //{
        //    float r, T;

        //    T = _animations[a.index].anim.length;

        //    if (a.startTime == a.endTime)
        //    {
        //        r = a.startTime;
        //        CompleteAnimation(r, a.endDone);
        //        return false;
        //    }

        //    var scaledElapsedTime = (Time.time - a.unityStartTime) * a.speed;

        //    if (a.startTime > a.endTime)
        //        scaledElapsedTime *= -1;

        //    r = scaledElapsedTime + a.startTime;

        //    var c1 = a.startTime < a.endTime && r >= a.stopTime && a.stopTime >= a.startTime && a.stopTime < a.endTime;
        //    var c2 = a.startTime > a.endTime && r <= a.stopTime && a.stopTime <= a.startTime && a.stopTime > a.endTime;

        //    if (c1 || c2)
        //    {
        //        r = a.stopTime;
        //        Util.Log($"Stopping Animation {a.index}.");
        //        CompleteAnimation(r, a.stopDone);
        //        return false;
        //    }

        //    var c3 = a.startTime < a.endTime && r >= a.endTime;
        //    var c4 = a.startTime > a.endTime && r <= a.endTime;

        //    if (c3 || c4)
        //    {
        //        r = a.endTime;
        //        Util.Log($"Done Animation {a.index}.");
        //        CompleteAnimation(r, a.endDone);
        //        return false;
        //    }

        //    SampleAnimationAtTime(r);

        //    return true;

        //    float GetTimeStamp(float r)
        //    {
        //        var s = r > 0 ? Mathf.Ceil((r - T) / T) : Mathf.Floor(r / T);
        //        return T == 0 ? 0 : r - s * T;
        //    }

        //    void SampleAnimationAtTime(float r)
        //    {
        //        var t = GetTimeStamp(r);
        //        _animations[a.index].playhead = t;
        //        _animations[a.index].virtualPlayhead = r;
        //        _animations[a.index].anim.time = t;
        //        animationComponent.Sample();
        //    }

        //    void CompleteAnimation(float t, Action callback)
        //    {
        //        SampleAnimationAtTime(t);
        //        StopAnimation(a.index);
        //        callback();
        //    }
        //}

        public void PlayuAudio(in AudioPlayData data)
        {
            if (data.clip != audioSource.clip)
            {
                audioSource.clip = data.clip;
            }
            audioSource.Play();
        }

        internal void StopAudio(in AudioPlayData data)
        {
            if (data.clip != audioSource.clip)
            {
                audioSource.clip = data.clip;
            }
            audioSource.Stop();
            _currentAudioData.stopTime = audioSource.time;
        }

        internal void PauseAudio(in AudioPlayData data)
        {
            if (data.clip != audioSource.clip)
            {
                audioSource.clip = data.clip;
            }
            audioSource.Pause();
            _currentAudioData.stopTime = audioSource.time;
        }

        public bool UnPauseAudio(in Audio
        {
            return _animationsInProgress.ContainsKey(index);
        }

        public float GetAnimationMaxTime(int index)
        {
            return _animations[index].anim.length;
        }

        public float GetPlayhead(int index)
        {
            return _animations[index].playhead;
        }

        public float GetVirtualPlayhead(int index)
        {
            return _animations[index].virtualPlayhead;
        }
    }
}