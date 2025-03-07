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
    }
}