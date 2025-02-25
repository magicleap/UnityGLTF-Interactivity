using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace UnityGLTF.Interactivity
{
    public static class EventsDeserializer
    {
        public static List<Customevent> GetEvents(JObject jObj)
        {
            var jEvents = jObj[ConstStrings.EVENTS].Children();
            var events = new List<Customevent>(jEvents.Count());

            foreach (var v in jEvents)
            {
                events.Add(new Customevent()
                {
                    id = v[ConstStrings.ID].Value<string>(),
                    values = GetEventValues(v[ConstStrings.VALUES] as JObject)
                });
            }

            return events;
        }

        private static List<EventValue> GetEventValues(JObject jValues)
        {
            var valueCount = jValues.Count;
            var values = new List<EventValue>(valueCount);

            foreach (var kvp in jValues)
            {
                values.Add(new EventValue()
                {
                    id = kvp.Key,
                    type = kvp.Value[ConstStrings.TYPE].Value<int>(),
                });
            }

            return values;
        }
    }
}