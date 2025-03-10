using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity.Extensions
{
    public static partial class ExtensionMethods
    {
        public static float3 ToFloat3(this Color c)
        {
            return new float3(c.r, c.g, c.b);
        }

        public static Color ToColor(this float3 v)
        {
            return new Color(v.x, v.y, v.z, 1f);
        }

        public static float4 ToFloat4(this Color c)
        {
            return new float4(c.r, c.g, c.b, c.a);
        }

        public static Color ToColor(this float4 v)
        {
            return new Color(v.x, v.y, v.z, v.w);
        }

        public static float4 ToFloat4(this quaternion q)
        {
            return q.value;
        }

        public static float4 ToFloat4(this Quaternion q)
        {
            return new float4(q.x, q.y, q.z, q.w);
        }

        public static quaternion ToQuaternion(this float4 v)
        {
            return new quaternion(v.x, v.y, v.z, v.w);
        }

        public static float3 SwapHandedness(this float3 v)
        {
            return new float3(-v.x, v.y, v.z);
        }

        public static Vector3 SwapHandedness(this Vector3 v)
        {
            return new Vector3(-v.x, v.y, v.z);
        }

        public static Quaternion SwapHandedness(this Quaternion q)
        {
            // TODO: Figure out if there's a way to do this without converting to euler angles and back as it's really slow.
            var euler = q.eulerAngles;

            euler.z *= -1;

            return Quaternion.Euler(euler);
        }
    }
}