using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static partial class Helpers
    {
        public static async Task InterpolateAsync<T>(T from, T to, Action<T> setter, Func<T, T, float, T> evaluator, float duration)
        {
            if (from is not T) throw new InvalidOperationException("Argument 'from' is not the given type!");
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                setter(evaluator(from, to, t));
                await Task.Yield();
            }
        }

        public static async Task InterpolateAsync<T>(T from, T to, Pointer<T> pointer, float duration)
        {
            if (from is not T) throw new InvalidOperationException("Argument 'from' is not the given type!");
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                pointer.setter(pointer.evaluator(from, to, t));
                await Task.Yield();
            }
        }

        public static async Task InterpolateAsync<T>(T to, Pointer<T> pointer, float duration)
        {
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            var from = pointer.getter();

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                pointer.setter(pointer.evaluator(from, to, t));
                await Task.Yield();
            }
        }

        public static async Task<bool> InterpolateAsync<T>(T from, T to, Action<T> setter, Func<T, T, float, T> evaluator, float duration, CancellationToken cancellationToken)
        {
            if (from is not T) throw new InvalidOperationException("Argument 'from' is not the given type!");
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                setter(evaluator(from, to, t));
                await Task.Yield();
            }

            return true;
        }

        public static async Task<bool> InterpolateAsync<T>(T from, T to, Pointer<T> pointer, float duration, CancellationToken cancellationToken)
        {
            if (from is not T) throw new InvalidOperationException("Argument 'from' is not the given type!");
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                pointer.setter(pointer.evaluator(from, to, t));
                await Task.Yield();
            }

            return true;
        }

        public static async Task<bool> InterpolateAsync<T>(T to, Pointer<T> pointer, float duration, CancellationToken cancellationToken)
        {
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            var from = pointer.getter();

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                pointer.setter(pointer.evaluator(from, to, t));
                await Task.Yield();
            }

            return true;
        }
    }
}