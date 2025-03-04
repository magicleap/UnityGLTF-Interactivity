using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public struct BezierInterpolateData
    {
        public IPointer pointer;
        public float duration;
        public Vector2 cp0;
        public Vector2 cp1;
        public InterpolateCancelToken cancellationToken;
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

        public static Vector2 CubicBezier(float t, Vector2 cp0, Vector2 cp1)
        {
            var omt = 1 - t;
            return 3f * t * omt * omt * cp0 + 3f * t * t * omt * cp1 + t * t * t * Vector2.one;
        }
    }
}