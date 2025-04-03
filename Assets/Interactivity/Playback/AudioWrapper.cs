using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityGLTF.Interactivity
{
    public class AudioPlayData
    {
        public int index;
        public UnityEngine.AudioSource source;
        public float stopTime;
        public float pauseTime;
        public AudioWrapper.AudioState state = AudioWrapper.AudioState.Stopped;
        public Action stopDone;
        public Action playDone;
        public Action pauseDone;
        public Action unpauseDone;
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
        public enum AudioState
        {
            Stopped,
            Playing,
            Paused,
        };

        private AudioPlayData _currentAudioData;

        private readonly Dictionary<int, AudioPlayData> _audioSources = new();

        private BehaviourEngine _engine;

        public void AddAudioSource(int index, AudioPlayData data)
        {
            //purge old source if it is there
            _audioSources[index] = data;
        }

        public void SetData(BehaviourEngine behaviourEngine)
        {
            //if (_engine != null)
            //    _engine.onTick -= OnTick;

            _engine = behaviourEngine;
            //_engine.onTick += OnTick;
//            _currentAudioData = data;
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

        public void PlayAudio(int index)
        {
            if (_audioSources.ContainsKey(index))
            {
                _audioSources[index].source.Play();
                _audioSources[index].pauseTime = 0;
                _audioSources[index].stopTime = 0;
                _audioSources[index].state = AudioState.Playing;
            }
            else
            {
                //?DPQ log
            }
        }

        public void StopAudio(int index)
        {
            if (_audioSources.ContainsKey(index))
            {
                _audioSources[index].source.Stop();
                _audioSources[index].stopTime = _audioSources[index].source.time;
                _audioSources[index].pauseTime = 0;
                _audioSources[index].state = AudioState.Stopped;
            }
            else { }
//                var v = _audioSources.Where(r => ((r.Value.source == data.source) && (r.Value.index == data.source)));
            //v.source.Pause();
            //v.stopTime = v.source.time;
            //v.pauseTime = 0;
            //v.state = AudioState.Stopped;
        }

        public void PauseAudio(int index)
        {
            //            var v = _audioSources.Find(r => ((r.clip == data.clip) && (r.source == data.source)));
            if (_audioSources.ContainsKey(index))
            {
                _audioSources[index].source.Pause();
                _audioSources[index].stopTime = 0;
                _audioSources[index].pauseTime = _audioSources[index].source.time;
                _audioSources[index].state = AudioState.Paused;
            }
            //v.source.Play();
            //v.pauseTime = v.source.time;
            //v.state = AudioState.Paused;
        }

        public void UnPauseAudio(int index)
        {
            var v = _audioSources.Where(r => ((r.Key == index))).FirstOrDefault();
            if (v.Value.state == AudioState.Paused)
            {
                v.Value.source.UnPause();
                v.Value.pauseTime = 0;
                v.Value.stopTime = 0;
            }
            else if (v.Value.state == AudioState.Stopped)
            {
                v.Value.source.Play();
                v.Value.pauseTime = 0;
                v.Value.stopTime = 0;
            }
            // if playing, let keep playing.
        }
    }
}