using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UnityGLTF.Interactivity
{
    public static class AudioEmittersDeserializer
    {
        public static List<AudioEmitter> GetAudioEmitters(JObject jObj)
        {
            var jAudioEmitter = jObj[ConstStrings.AUDIO_EMITTERS].Children();
            var audioEmittersList = new List<AudioEmitter>(jAudioEmitter.Count());

            foreach (var v in jAudioEmitter)
            {
                var audioEmitter = new AudioEmitter();
                audioEmitter.name = v[ConstStrings.NAME]?.Value<string>();
                audioEmitter.type = v[ConstStrings.AUDIO_TYPE]?.Value<string>();
                if (v[ConstStrings.GAIN] != null)
                    audioEmitter.gain = (float)v[ConstStrings.GAIN]?.Value<float>();

                audioEmitter.sources = new List<int>();
                JToken sourcesToken = v[ConstStrings.AUDIO_SOURCES];
                foreach (var s in sourcesToken)
                {
                    int sourceIdx = s.Value<int>();
                    audioEmitter.sources.Add(sourceIdx);
                }

                audioEmitter.positional = new List<PositionAudioData>();
                JToken positionalToken = v[ConstStrings.POSITIONAL];
//                foreach(var p in positionalToken)
                {
                    PositionAudioData pad = new PositionAudioData();
                    var token = positionalToken[ConstStrings.DISTANCE_MODEL];
                    pad.distanceModel = positionalToken[ConstStrings.DISTANCE_MODEL]?.Value<string>();
                    if (v[ConstStrings.MIN_DISTANCE] != null)
                        pad.minDistance = (float)positionalToken[ConstStrings.MIN_DISTANCE]?.Value<float>();
                    if (v[ConstStrings.MAX_DISTANCE] != null)
                        pad.maxDistance = (float)positionalToken[ConstStrings.MAX_DISTANCE]?.Value<float>();

                    audioEmitter.positional.Add(pad);
                }
                audioEmittersList.Add(audioEmitter);
            }

            return audioEmittersList;
        }
    }
}
