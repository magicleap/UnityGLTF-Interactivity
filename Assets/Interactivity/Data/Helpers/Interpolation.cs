using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static partial class Helpers
    {
        public static async Task<bool> InterpolateAsync<T>(T from, T to, Action<T> setter, Func<T, T, float, T> evaluator, float duration, CancellationToken cancellationToken)
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                setter(evaluator(from, to, t));
                await Task.Yield();
            }

            return true;
        }

        public static async Task<bool> LinearInterpolateAsync<T>(T to, Pointer<T> pointer, float duration, CancellationToken cancellationToken)
        {
            return await InterpolateAsync(pointer.getter(), to, pointer.setter, pointer.evaluator, duration, cancellationToken);
        }

        public static async Task<bool> InterpolateBezierAsync<T>(T to, Pointer<T> pointer, float duration, Vector2 cp0, Vector2 cp1, CancellationToken cancellationToken)
        {
            var evaluator = new Func<T, T, float, T>((a, b, t) => pointer.evaluator(a, b, CubicBezier(t, cp0, cp1).y));

            return await InterpolateAsync(pointer.getter(), to, pointer.setter, evaluator, duration, cancellationToken);
        }

        public static Vector2 CubicBezier(float t, Vector2 cp0, Vector2 cp1)
        {
            var omt = 1 - t;
            return 3f * t * omt * omt * cp0 + 3f * t * t * omt * cp1 + t * t * t * Vector2.one;
        }
    }
}