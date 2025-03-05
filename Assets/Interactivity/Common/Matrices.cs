using UnityEngine;

namespace UnityGLTF.Interactivity.Extensions
{
    public static class MatrixExtensions
    {
        private const int MATRIX4X4_SIZE = 16;

        public static Matrix4x4 LerpToComponentwise(this Matrix4x4 from, Matrix4x4 to, float t)
        {
            var m = new Matrix4x4();
            for (int i = 0; i < MATRIX4X4_SIZE; i++)
            {
                m[i] = Mathf.Lerp(from[i], to[i], t);
            }

            return m;
        }
    }
}