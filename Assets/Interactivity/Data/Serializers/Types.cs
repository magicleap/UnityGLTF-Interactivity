using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public static class TypesSerializer
    {
        public static Dictionary<Type, int> GetSystemTypeByIndexDictionary(Graph value)
        {
            var typeIndexByType = new Dictionary<Type, int>();

            for (int i = 0; i < value.types.Count; i++)
            {
                typeIndexByType.Add(Helpers.GetSystemType(value.types[i]), i);
            }

            return typeIndexByType;
        }

        public static void WriteJson(JsonWriter writer, List<InteractivityType> types)
        {
            writer.WritePropertyName(ConstStrings.TYPES);
            writer.WriteStartArray();

            for (int i = 0; i < types.Count; i++)
            {
                WriteType(writer, types[i]);
            }

            writer.WriteEndArray();
        }

        private static void WriteType(JsonWriter writer, InteractivityType type)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(ConstStrings.SIGNATURE);
            writer.WriteValue(type.signature);

            // TODO: Add support for type extensions here.

            writer.WriteEndObject();
        }
    }
}