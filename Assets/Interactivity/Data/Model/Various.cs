using Newtonsoft.Json;
using System;

namespace UnityGLTF.Interactivity
{
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
        public object value { get; set; }
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