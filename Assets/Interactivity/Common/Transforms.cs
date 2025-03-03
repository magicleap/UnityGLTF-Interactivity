using UnityEngine;

namespace UnityGLTF.Interactivity.Extensions
{
    public static class TransformExtensions
    {
        private static void GetTRS(this Matrix4x4 matrix, out Vector3 translation, out Quaternion rotation, out Vector3 scale)
        {
            float det = matrix.GetDeterminant();

            // Scale
            scale.x = matrix.MultiplyVector(new Vector3(1, 0, 0)).magnitude;
            scale.y = matrix.MultiplyVector(new Vector3(0, 1, 0)).magnitude;
            scale.z = matrix.MultiplyVector(new Vector3(0, 0, 1)).magnitude;
            scale = (det < 0) ? -scale : scale;

            // Rotation
            Matrix4x4 rotationMatrix = matrix;
            rotationMatrix.m03 = rotationMatrix.m13 = rotationMatrix.m23 = 0f;
            rotationMatrix = rotationMatrix * new Matrix4x4 { m00 = 1f / scale.x, m11 = 1f / scale.y, m22 = 1f / scale.z, m33 = 1 };
            rotation = Quaternion.LookRotation(rotationMatrix.GetColumn(2), rotationMatrix.GetColumn(1));

            // Position
            translation = matrix.GetColumn(3);
        }

        private static float GetDeterminant(this Matrix4x4 matrix)
        {
            return matrix.m00 * (matrix.m11 * matrix.m22 - matrix.m12 * matrix.m21) -
                    matrix.m10 * (matrix.m01 * matrix.m22 - matrix.m02 * matrix.m21) +
                    matrix.m20 * (matrix.m01 * matrix.m12 - matrix.m02 * matrix.m11);
        }

        public static void SetFromLocalMatrix(this Transform transform, Matrix4x4 matrix, bool isRightHanded)
        {
            matrix.GetTRS(out Vector3 t, out Quaternion r, out Vector3 s);

            if (isRightHanded)
            {
                t = new Vector3(-t.x, t.y, t.z);
                r = new Quaternion(-r.x, r.y, r.z, -r.w);
            }

            // Assign local TRS to transform
            transform.localPosition = t;
            transform.localRotation = r;
            transform.localScale = s;
        }

        public static void SetFromWorldMatrix(this Transform transform, Matrix4x4 matrix, bool isRightHanded)
        {
            matrix.GetTRS(out Vector3 t, out Quaternion r, out Vector3 s);

            if (isRightHanded)
            {
                t = new Vector3(-t.x, t.y, t.z);
                r = new Quaternion(-r.x, r.y, r.z, -r.w);
            }

            // Assign world TRS to transform
            transform.position = t;
            transform.rotation = r;
            transform.SetGlobalScale(s);
        }

        public static Matrix4x4 GetLocalMatrix(this Transform transform, bool isRightHanded)
        {
            Vector3 t = transform.localPosition;
            Quaternion r = transform.localRotation;
            Vector3 s = transform.localScale;

            if (isRightHanded)
            {
                t = new Vector3(-t.x, t.y, t.z);
                r = new Quaternion(-r.x, r.y, r.z, -r.w);
            }

            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.SetTRS(t, r, s);

            return matrix;
        }

        public static Matrix4x4 GetWorldMatrix(this Transform transform, bool isRightHanded)
        {
            Vector3 t = transform.position;
            Quaternion r = transform.rotation;
            Vector3 s = transform.lossyScale;

            if (isRightHanded)
            {
                t = new Vector3(-t.x, t.y, t.z);
                r = new Quaternion(-r.x, r.y, r.z, -r.w);
            }

            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.SetTRS(t, r, s);

            return matrix;
        }

        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }
    }
}