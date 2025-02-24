using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static class Parser
    {
        public static float ToFloat(JArray jArray)
        {
            return jArray[0].Value<float>();
        }

        public static int ToInt(JArray jArray)
        {
            return jArray[0].Value<int>();
        }

        public static bool ToBool(JArray jArray)
        {
            return jArray[0].Value<bool>();
        }

        public static string ToString(JArray jArray)
        {
            return jArray[0].Value<string>();
        }

        public static Vector2 ToVector2(JArray jArray)
        {
            return new Vector2(jArray[0].Value<float>(), jArray[1].Value<float>());
        }

        public static Vector3 ToVector3(JArray jArray)
        {
            return new Vector3(jArray[0].Value<float>(), jArray[1].Value<float>(), jArray[2].Value<float>());
        }

        public static Vector4 ToVector4(JArray jArray)
        {
            return new Vector4(jArray[0].Value<float>(), jArray[1].Value<float>(), jArray[2].Value<float>(), jArray[3].Value<float>());
        }

        public static int[] ToIntArray(JArray jArray)
        {
            var arr = new int[jArray.Count];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = jArray[i].Value<int>();
            }

            return arr;
        }

        public static Matrix4x4 ToMatrix4x4(JArray jArray)
        {
            const int MATRIX_SIZE = 16;

            var m = new Matrix4x4();

            // Unity and GLTF both use Column-Major matrices so we can do a 1:1 transfer.
            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                m[i] = jArray[i].Value<int>();
            }

            return m;
        }
    }
}