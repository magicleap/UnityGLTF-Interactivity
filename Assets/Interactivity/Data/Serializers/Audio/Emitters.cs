using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UnityGLTF.Interactivity
{
    public static class AudioEmittersDeserializer
    {
        public static List<AudioEmitter> GetAudioEmitters<T>(T t, JObject jObj)
        {
            var jAudioEmitter = jObj[ConstStrings.AUDIO_EMITTERS].Children();
            var audioEmittersList = new List<AudioEmitter>(jAudioEmitter.Count());

            foreach (var v in jAudioEmitter)
            {
                var audioEmitter = new AudioEmitter();

                if (t is KHR_AudioType)
                {
                    audioEmitter.name = v[ConstStrings.NAME]?.Value<string>();
                    audioEmitter.type = v[ConstStrings.AUDIO_TYPE]?.Value<string>();

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
                }
                else if (t is GOOG_AudioType)
                {
                    if (v.HasValues)
                    {
                        audioEmitter.audio = v[ConstStrings.AUDIO].Value<int>();
                        if (v[ConstStrings.GAIN] != null)
                            audioEmitter.gain = (float)v[ConstStrings.GAIN]?.Value<float>();
                        if (v[ConstStrings.AUTO_PLAY] != null)
                            audioEmitter.autoPlay = (bool)v[ConstStrings.AUTO_PLAY]?.Value<bool>();
                        if (v[ConstStrings.LOOP] != null)
                            audioEmitter.loop = (bool)v[ConstStrings.LOOP]?.Value<bool>();
                    }
                    else
                    {
                        // this is part of the interactivity graph.
                        /// no op
                        return null;
                    }
                }
                audioEmittersList.Add(audioEmitter);
            }

            return audioEmittersList;
        }
    }
}
