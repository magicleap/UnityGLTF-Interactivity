using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UnityGLTF.Interactivity
{
    public static class VideoSourceDeserializer
    {
        public static List<VideoSource> GetVideoSources(JObject jObj)
        {
            var jVideoSource = jObj[ConstStrings.VIDEO_SOURCE].Children();
            var videoSourcesList = new List<VideoSource>(jVideoSource.Count());

            foreach (var v in jVideoSource)
            {
                var videoSource = new VideoSource();
                if (v.HasValues)
                {
                    if (v[ConstStrings.MIME_TYPE] != null)
                        videoSource.mimeType = v[ConstStrings.MIME_TYPE]?.Value<string>();
                    if (v[ConstStrings.BUFFER_VIEW] != null)
                        videoSource.bufferView = v[ConstStrings.BUFFER_VIEW].Value<int>();
                    if (v[ConstStrings.URI] != null)
                        videoSource.uri = v[ConstStrings.URI]?.Value<string>();

                    videoSourcesList.Add(videoSource);
                }
            }

            return videoSourcesList;
        }
    }
}
