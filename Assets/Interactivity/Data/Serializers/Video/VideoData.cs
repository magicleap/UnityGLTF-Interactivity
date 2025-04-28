using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityGLTF.Interactivity
{
    public static class VideoDataDeserializer
    {
        public static List<VideoData> GetVideoData(JObject jObj)
        {
            var jVideoData = jObj[ConstStrings.VIDEO_DATA].Children();

            if (jVideoData[ConstStrings.VIDEO_DATA] == null)
                return null;
            var videoList = new List<VideoData>(jVideoData.Count());

            foreach (var v in jVideoData)
            {
                var videoData = new VideoData();
                videoData.autoPlay = v[ConstStrings.AUTO_PLAY].Value<bool>();
                videoData.loop = v[ConstStrings.LOOP].Value<bool>();
                videoData.playHead= v[ConstStrings.PLAYHEAD].Value<int>();
                videoData.source = v[ConstStrings.SOURCE].Value<int>();

                videoList.Add(videoData);
            }

            return videoList;
        }
    }
}
