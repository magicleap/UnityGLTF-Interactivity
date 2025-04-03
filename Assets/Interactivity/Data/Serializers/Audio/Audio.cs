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
                if (v[ConstStrings.MIME_TYPE] != null)
                    audio.mimeType = v[ConstStrings.MIME_TYPE]?.Value<string>();
                if (v[ConstStrings.BUFFER_VIEW] != null)
                    audio.bufferView = v[ConstStrings.BUFFER_VIEW].Value<int>();
                if (v[ConstStrings.URI] != null)
                    audio.uri = v[ConstStrings.URI]?.Value<string>();

                audioList.Add(audio);
            }

            return audioList;
        }
    }
}
