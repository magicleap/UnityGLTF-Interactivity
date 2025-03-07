using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public struct BezierInterpolateData
    {
        public IPointer pointer;
        public float duration;
        public Vector2 cp0;
        public Vector2 cp1;
        public NodeEngineCancelToken cancellationToken;
    }

    public static partial class Helpers
    {
        public static async Task<bool> InterpolateAsync<T,V>(T from, T to, Action<T> setter, Func<T, T, float, T> evaluator, float duration, V cancellationToken) where V : struct, ICancelToken
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                if (cancellationToken.isCancelled)
                    return false;

                setter(evaluator(from, to, t));
                await Task.Yield();
            }

            return true;
        }

        public static async Task<bool> LinearInterpolateAsync<T,V>(T to, Pointer<T> pointer, float duration, V cancellationToken) where V : struct, ICancelToken
        {
            return await InterpolateAsync(pointer.getter(), to, pointer.setter, pointer.evaluator, duration, cancellationToken);
        }

        public static async Task<bool> InterpolateBezierAsync<T>(Property<T> to, BezierInterpolateData d)
        {
            var v = to.value;
            return await InterpolateBezierAsync(v, d);
        }

        public static async Task<bool> InterpolateBezierAsync<T>(T to, BezierInterpolateData d)
        {
            var p = (Pointer<T>)d.pointer;
            
            var evaluator = new Func<T, T, float, T>((a, b, t) => p.evaluator(a, b, CubicBezier(t, d.cp0, d.cp1).y));

            return await InterpolateAsync(p.getter(), to, p.setter, evaluator, d.duration, d.cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CubicBezier(float t, Vector2 cp0, Vector2 cp1)
        {
            var omt = 1 - t;
            return 3f * t * omt * omt * cp0 + 3f * t * t * omt * cp1 + t * t * t * Vector2.one;
        }

        public static float4 nlerp(float4 q1, float4 q2, float t)
        {
            float dt = math.dot(q1, q2);
            if (dt < 0.0f)
            {
                q2 = -q2;
            }
            
            return math.normalize(math.lerp(q1, q2, t));
        }

        public static float4 SlerpVector4(float4 q1, float4 q2, float t)
        {
            float dt = math.dot(q1, q2);
            if (dt < 0.0f)
            {
                dt = -dt;
                q2 = -q2;
            }

            if (dt < 0.9995f)
            {
                float angle = math.acos(dt);
                float s = math.rsqrt(1.0f - dt * dt);    // 1.0f / sin(angle)
                float w1 = math.sin(angle * (1.0f - t)) * s;
                float w2 = math.sin(angle * t) * s;
                return q1 * w1 + q2 * w2;
            }
            else
            {
                // if the angle is small, use linear interpolation
                return nlerp(q1, q2, t);
            }
        }
    }
}