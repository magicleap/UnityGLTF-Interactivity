using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public static class EventsSerializer
    {
        public static void WriteJson(JsonWriter writer, List<Customevent> events, Dictionary<Type, int> typeIndexByType)
        {
            writer.WritePropertyName(ConstStrings.EVENTS);
            writer.WriteStartArray();

            for (int i = 0; i < events.Count; i++)
            {
                WriteEvent(writer, events[i], typeIndexByType);
            }

            writer.WriteEndArray();
        }

        private static void WriteEvent(JsonWriter writer, Customevent customevent, Dictionary<Type, int> typeIndexByType)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.ID);
            writer.WriteValue(customevent.id);

            WriteEventValues(writer, customevent.values, typeIndexByType);

            writer.WriteEndObject();
        }

        private static void WriteEventValues(JsonWriter writer, List<EventValue> events, Dictionary<Type, int> typeIndexByType)
        {
            writer.WritePropertyName(ConstStrings.VALUES);
            writer.WriteStartObject();

            for (int i = 0; i < events.Count; i++)
            {
                WriteEventValue(writer, events[i], typeIndexByType);
            }

            writer.WriteEndObject();
        }

        private static void WriteEventValue(JsonWriter writer, EventValue eventValue, Dictionary<Type, int> typeIndexByType)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(eventValue.id);

            writer.WritePropertyName(ConstStrings.TYPE);
            writer.WriteValue(typeIndexByType[eventValue.property.GetType()]);

            writer.WriteEndObject();
        }
    }
}