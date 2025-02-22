using UnityEngine;

namespace UnityGLTF.Interactivity.Extensions
{
    public static partial class ExtensionMethods
    {
        public static Vector3 ToVector3(this Color c)
        {
            return new Vector3(c.r, c.g, c.b);
        }

        public static Color ToColor(this Vector3 v)
        {
            return new Color(v.x, v.y, v.z, 1f);
        }

        public static Vector4 ToVector4(this Color c)
        {
            return new Vector4(c.r, c.g, c.b, c.a);
        }

        public static Color ToColor(this Vector4 v)
        {
            return new Color(v.x, v.y, v.z, v.w);
        }

        public static Vector4 ToVector4(this Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

        public static Quaternion ToQuaternion(this Vector4 v)
        {
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        public static Vector3 SwapHandedness(this Vector3 v)
        {
            return new Vector3(-v.x, v.y, v.z);
        }

        public static Quaternion SwapHandedness(this Quaternion q)
        {
            // TODO: Figure out if there's a way to do this without converting to euler angles and back as it's pretty slow.
            var euler = q.eulerAngles;

            euler.z *= -1;

            return Quaternion.Euler(euler);
        }
    }
}