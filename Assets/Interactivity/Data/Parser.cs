using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static class Parser
    {
        private static T Parse<T>(object o)
        {
            var sanitized = SanitizeObjectString(o);

            return (T)Convert.ChangeType(sanitized, typeof(T));
        }

        private static string SanitizeObjectString(object o)
        {
            return String.Join("", o.ToString().Split('[', ']', '\n', '\r', ' ', '\"'));
        }

        public static int ToInt(object o)
        {
            return Parse<int>(o);
        }

        public static float ToFloat(object o)
        {
            return Parse<float>(o);
        }

        public static Vector2 ToVector2(object o)
        {
            var sanitized = SanitizeObjectString(o).Split(',');

            if (sanitized.Length != 2)
                throw new InvalidOperationException($"Json property is not a vector2!\nValue: {o.ToString()}");

            return new Vector2(float.Parse(sanitized[0]), float.Parse(sanitized[1]));
        }

        public static Vector3 ToVector3(object o)
        {
            var sanitized = SanitizeObjectString(o).Split(',');

            if (sanitized.Length != 3)
                throw new InvalidOperationException($"Json property is not a vector3!\nValue: {o.ToString()}");

            return new Vector3(float.Parse(sanitized[0]), float.Parse(sanitized[1]), float.Parse(sanitized[2]));
        }

        public static Vector4 ToVector4(object o)
        {
            var sanitized = SanitizeObjectString(o).Split(',');

            if (sanitized.Length != 4)
                throw new InvalidOperationException($"Json property is not a vector4!\nValue: {o.ToString()}");

            return new Vector4(float.Parse(sanitized[0]), float.Parse(sanitized[1]), float.Parse(sanitized[2]), float.Parse(sanitized[3]));
        }

        public static string ToString(object o)
        {
            return SanitizeObjectString(o);
        }

        public static int[] ToIntArray(object o)
        {
            var sanitized = SanitizeObjectString(o).Split(',');

            var array = new int[sanitized.Length];

            for (int i = 0; i < sanitized.Length; i++)
            {
                try
                {
                    array[i] = int.Parse(sanitized[i]);
                }
                catch
                {
                    Debug.LogError($"Element {i} in int array with value {sanitized[i]} could not be parsed!");
                }
            }
            
            return array;
        }

        public static bool ToBool(object o)
        {
            var sanitized = String.Join("", o.ToString().Split('[', ']', '\n', '\r', ' ', '\"')).ToLower();

           return sanitized.Contains("true");
        }
    }
}