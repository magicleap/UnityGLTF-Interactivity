using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public class KHR_ExtensionGraph
    {
        public enum GraphType
        {
            Interactivity,
            Audio
        }
        public List<Graph> graphs { get; set; } = new();
        public int defaultGraphIndex { get; set; }
        public GraphType type { get; set; } = GraphType.Interactivity;
    }

    //public class KHR_audio_emitter
    //{
    //    public List<Graph> audioElementsGraphs { get; set; } = new();
    //    public int defaultGraphIndex { get; set; }
    //}

    public class Audio
    {
        public string mimeType { get; set; }
        public int bufferView { get; set; }
        public string uri { get; set; }
    }

    public class AudioSource
    {
        public bool autoPlay { get; set; }
        public float gain { get; set; }
        public bool loop { get; set; }
        public int audio { get; set; }
        public string sourceName { get; set; }
    }
    
    public class AudioEmitter
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<int> sources { get; set; }
        public float gain { get; set; }
        public List<PositionAudioData> positional { get; set; }
    }

    public class AudioEmitterPartial
    {
        public string name { get; set;  }
        public string type { get; set; }
        public int source { get; set; }
        public float gain { get; set; }
        public PositionAudioData positional { get; set; }
    }

    public class PositionAudioData
    {
        public string distanceModel { get; set; }
        public float minDistance { get; set; }
        public float maxDistance { get; set; }
    }

    public class Declaration
    {
        public string op { get; set; }
        public string extension { get; set; }
        public List<ValueSocket> inputValueSockets { get; set; }
        public List<ValueSocket> outputValueSockets { get; set; }
    }

    public class ValueSocket
    {
        public ValueSocket(string name, int type)
        {
            this.name = name;
            this.type = type;
        }

        public string name { get; set; }
        public int type { get; set; }
    }

    public class Metadata
    {
        public double positionX { get; set; }
        public double positionY { get; set; }
    }

    public class Variable
    {
        public string id { get; set; }
        public IProperty property { get; set; }
        public IProperty initialValue { get; set; }
    }

    public class Configuration
    {
        public string id { get; set; }
        public JArray value { get; set; }
    }

    public class InteractivityType
    {
        public string signature { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TypeExtensions extensions { get; set; }
    }

    public class TypeExtensions
    {
        public AMZN_Interactivity_String AMZN_interactivity_string { get; set; }
    }

    public class AMZN_Interactivity_String
    {
    }
}