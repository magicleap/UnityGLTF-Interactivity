using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace UnityGLTF.Interactivity
{
    public static class AudioSourceDeserializer
    {
        public static List<AudioSource> GetAudioSources(JObject jObj)
        {
            var jAudioSources = jObj[ConstStrings.AUDIO_SOURCES].Children();
            var audioSourcesList = new List<AudioSource>(jAudioSources.Count());

            foreach (var v in jAudioSources)
            {
                AudioSource audioSource = new AudioSource();
                audioSource.autoPlay = (bool)(v[ConstStrings.AUTO_PLAY]?.Value<bool>());
                audioSource.gain = (float)(v[ConstStrings.GAIN]?.Value<float>());
                audioSource.loop = (bool)(v[ConstStrings.LOOP]?.Value<bool>());
                audioSource.sourceName = v[ConstStrings.SOURCE_NAME]?.Value<string>();

                audioSourcesList.Add(audioSource);
            }

            return audioSourcesList;
        }
    }
}
