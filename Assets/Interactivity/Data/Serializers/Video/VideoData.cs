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
            var videoList = new List<VideoData>(jVideoData.Count());

            foreach (var v in jVideoData)
            {
                var videoData = new VideoData();
                if (v[ConstStrings.NAME] != null)
                    videoData.name = v[ConstStrings.NAME]?.Value<string>();
                videoData.autoPlay = v[ConstStrings.AUTO_PLAY].Value<bool>();
                videoData.loop = v[ConstStrings.LOOP].Value<bool>();
                videoData.speed = v[ConstStrings.SPEED].Value<float>();
                videoData.video = v[ConstStrings.VIDEO].Value<int>();

                videoList.Add(videoData);
            }

            return videoList;
        }
    }
}
