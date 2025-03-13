using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity.Extensions
{
    public static class MatrixExtensions
    {
        public static float4x4 LerpToComponentwise(this float4x4 from, float4x4 to, float t)
        {
            var c0 = math.lerp(from.c0, to.c0, t);
            var c1 = math.lerp(from.c1, to.c1, t);
            var c2 = math.lerp(from.c2, to.c2, t);
            var c3 = math.lerp(from.c3, to.c3, t);

            var m = new float4x4(c0, c1, c2, c3);

            return m;
        }

        /// <summary>
        /// Added because Unity's Mathematics library has a bug where float4x4.TRS creates a TSR matrix.
        /// </summary>
        public static float4x4 TRS(float3 translation, quaternion rotation, float3 scale)
        {
            float3x3 m = math.mul(Unity.Mathematics.float3x3.Scale(scale), new float3x3(rotation));
            return new float4x4(new float4(m.c0, 0.0f),
            new float4(m.c1, 0.0f),
            new float4(m.c2, 0.0f),
            new float4(translation, 1.0f));
        }
    }
}