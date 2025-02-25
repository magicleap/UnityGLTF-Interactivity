using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public class EventValue
    {
        public string id { get; set; }
        public int type { get; set; }
    }

    public class Customevent
    {
        public string id { get; set; }
        public List<EventValue> values { get; set; } = new();
    }
}