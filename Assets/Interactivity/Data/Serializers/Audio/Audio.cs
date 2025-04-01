using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityGLTF.Interactivity
{
    public static class AudioDeserializer
    {
        public static List<Audio> GetAudio(JObject jObj)
        {
            var jAudio = jObj[ConstStrings.AUDIO].Children();
            var audioList = new List<Audio>(jAudio.Count());

            foreach (var v in jAudio)
            {
                var audio = new Audio();
                audio.mimeType = v[ConstStrings.MIME_TYPE]?.Value<string>();
                audio.bufferView = v[ConstStrings.BUFFER_VIEW]?.Value<string>();
                audio.uri = v[ConstStrings.URI]?.Value<string>();

                audioList.Add(audio);
            }

            return audioList;
        }
    }
}
