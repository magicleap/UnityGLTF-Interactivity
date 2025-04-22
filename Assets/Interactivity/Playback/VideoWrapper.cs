using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityGLTF.Interactivity
{
    public class VideoPlayerData
    {
        public int index;
        public UnityEngine.Video.VideoPlayer player;
        public double stopTime;
        public double pauseTime;
        public VideoWrapper.VideoState state = VideoWrapper.VideoState.Stopped;
        public Action actionDone;
    }


    public class VideoWrapper : MonoBehaviour
    {
        public enum VideoState
        {
            Stopped,
            Playing,
            Paused,
        };

        private VideoPlayerData _currentVideoData;

        private readonly Dictionary<int, VideoPlayerData> _videoSources = new();

        private BehaviourEngine _engine;

        public void AddVideoSource(int index, VideoPlayerData data)
        {
            //purge old source if it is there
            _videoSources[index] = data;
        }

        public void SetData(BehaviourEngine behaviourEngine)
        {
            _engine = behaviourEngine;
        }

        public void PlayVideo(int index)
        {
            if (_videoSources.ContainsKey(index))
            {
                _videoSources[index].player.Play();
                _videoSources[index].pauseTime = 0;
                _videoSources[index].stopTime = 0;
                _videoSources[index].state = VideoState.Playing;
            }
            else
            {
                //?DPQ log
            }
        }

        public void StopVideo(int index)
        {
            if (_videoSources.ContainsKey(index))
            {
                _videoSources[index].player.Stop();
                _videoSources[index].stopTime = _videoSources[index].player.clockTime;
                _videoSources[index].pauseTime = 0;
                _videoSources[index].state = VideoState.Stopped;
            }
            else { }
        }

        public void PauseVideo(int index)
        {
            if (_videoSources.ContainsKey(index))
            {
                _videoSources[index].player.Pause();
                _videoSources[index].stopTime = 0;
                _videoSources[index].pauseTime = _videoSources[index].player.clockTime;
                _videoSources[index].state = VideoState.Paused;
            }
        }

        public void UnPauseVideo(int index)
        {
            var v = _videoSources.Where(r => ((r.Key == index))).FirstOrDefault();
//            if (v.Value.state == VideoState.Paused)
            {
                v.Value.player.Play();
                v.Value.pauseTime = 0;
                v.Value.stopTime = 0;
                v.Value.state = VideoState.Playing;
            }
            //else if (v.Value.state == VideoState.Stopped)
            //{
            //    v.Value.source.Play();
            //    v.Value.pauseTime = 0;
            //    v.Value.stopTime = 0;
            //}
            //// if playing, let keep playing.
        }
    }
}