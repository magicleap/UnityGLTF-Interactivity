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
                    values = GetEventValues(v[ConstStrings.VALUES])
                });
            }

            return events;
        }

        private static List<EventValue> GetEventValues(JToken jToken)
        {
            var valueCount = jToken.Count();
            var values = new List<EventValue>(valueCount);

            foreach (var value in jToken)
            {
                values.Add(new EventValue()
                {
                    id = value[ConstStrings.ID].Value<string>(),
                    type = value[ConstStrings.TYPE].Value<int>(),
                    description = value[ConstStrings.DESCRIPTION].Value<string>(),
                });
            }

            return values;
        }
    }
}