using Newtonsoft.Json;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public static class EventsSerializer
    {
        public static void WriteJson(JsonWriter writer, List<Customevent> events)
        {
            writer.WritePropertyName(ConstStrings.EVENTS);
            writer.WriteStartArray();

            for (int i = 0; i < events.Count; i++)
            {
                WriteEvent(writer, events[i]);
            }

            writer.WriteEndArray();
        }

        private static void WriteEvent(JsonWriter writer, Customevent customevent)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.ID);
            writer.WriteValue(customevent.id);

            WriteEventValues(writer, customevent.values);

            writer.WriteEndObject();
        }

        private static void WriteEventValues(JsonWriter writer, List<EventValue> events)
        {
            writer.WritePropertyName(ConstStrings.VALUES);
            writer.WriteStartArray();

            for (int i = 0; i < events.Count; i++)
            {
                WriteEventValue(writer, events[i]);
            }

            writer.WriteEndArray();
        }

        private static void WriteEventValue(JsonWriter writer, EventValue eventValue)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.ID);
            writer.WriteValue(eventValue.id);

            writer.WritePropertyName(ConstStrings.TYPE);
            writer.WriteValue(eventValue.type);

            writer.WritePropertyName(ConstStrings.DESCRIPTION);
            writer.WriteValue(eventValue.description);

            writer.WriteEndObject();
        }
    }
}