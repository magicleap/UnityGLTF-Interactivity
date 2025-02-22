using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static class Parser
    {
        private static readonly char[] _avoidCharacters = new[] { '[', ']', '\n', '\r', ' ', '\"', ',' };

        private static StringSpanReader GetSpanReader(object o)
        {
            return new StringSpanReader(o.ToString(), _avoidCharacters);
        }

        public static int ToInt(object o)
        {
            var reader = GetSpanReader(o);

            reader.Slice();

            return int.Parse(reader.AsReadOnlySpan());
        }

        public static float ToFloat(object o)
        {
            var reader = GetSpanReader(o);

            reader.Slice();

            return float.Parse(reader.AsReadOnlySpan());
        }

        public static Vector2 ToVector2(object o)
        {
            const int VEC_SIZE = 2;
            Span<float> vec = stackalloc float[VEC_SIZE];

            ParseFloatArray(o, vec);

            return new Vector2(vec[0], vec[1]);
        }

        public static Vector3 ToVector3(object o)
        {
            const int VEC_SIZE = 3;
            Span<float> vec = stackalloc float[VEC_SIZE];

            ParseFloatArray(o, vec);

            return new Vector3(vec[0], vec[1], vec[2]);
        }

        public static Vector4 ToVector4(object o)
        {
            const int VEC_SIZE = 4;
            Span<float> vec = stackalloc float[VEC_SIZE];

            ParseFloatArray(o, vec);

            return new Vector4(vec[0], vec[1], vec[2], vec[3]);
        }

        public static Matrix4x4 ToMatrix4x4(object o)
        {
            const int MATRIX_4X4_LENGTH = 16;
            Span<float> vec = stackalloc float[MATRIX_4X4_LENGTH];

            ParseFloatArray(o, vec);

            var matrix = new Matrix4x4();

            // Unity matrices are column-major just like the GLTF Interactivity Spec float4x4.
            // This means we can just iterate through the matrix and parse floats without any index conversion.
            for (int i = 0; i < MATRIX_4X4_LENGTH; i++)
            {
                matrix[i] = vec[i];
            }

            return matrix;
        }

        public static int[] ToIntArray(object o)
        {
            var reader = GetSpanReader(o);
            var length = reader.CountCharacter(',') + 1;
            var array = new int[length];

            for (int i = 0; i < length; i++)
            {
                reader.FindFirstValidCharacter();
                reader.SetEndIndexByCharacters();

                array[i] = int.Parse(reader.AsReadOnlySpan());

                reader.SetStartIndexToEndIndex();
            }

            return array;
        }

        public static string ToString(object o)
        {
            var reader = GetSpanReader(o);
            reader.GetFirstQuotedSubstring();

            return reader.ToString();
        }

        public static bool ToBool(object o)
        {
            var reader = GetSpanReader(o);

            reader.Slice();

            return bool.Parse(reader.AsReadOnlySpan());
        }

        private static void ParseFloatArray(object o, Span<float> array)
        {
            var reader = GetSpanReader(o);

            var length = reader.CountCharacter(',') + 1;

            if (length != array.Length)
                throw new InvalidOperationException($"There are {length} floats from the json but only {array.Length} were requested!");

            for (int i = 0; i < length; i++)
            {
                reader.FindFirstValidCharacter();
                reader.SetEndIndexByCharacters();

                array[i] = float.Parse(reader.AsReadOnlySpan());

                reader.SetStartIndexToEndIndex();
            }
        }
    }
}