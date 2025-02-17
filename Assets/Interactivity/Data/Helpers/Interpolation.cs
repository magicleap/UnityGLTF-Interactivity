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

        public static void Interpolate<T>(T from, T to, Action<T> setter, Func<T, T, float, T> evaluator, float t)
        {
            if (from is not T) throw new InvalidOperationException("Argument 'from' is not the given type!");
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            setter(evaluator(from, to, t));
        }

        public static void Interpolate<T>(T from, T to, Pointer<T> pointer, float t)
        {
            if (from is not T) throw new InvalidOperationException("Argument 'from' is not the given type!");
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            pointer.setter(pointer.evaluator(from, to, t));
        }

        public static void Interpolate<T>(ref IValue initialValue, T to, Pointer<T> pointer, float t)
        {
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            if(initialValue == null)
            {
                var p = new Value<T>();
                p.value = pointer.GetValue();
                initialValue = p;
            }

            T from = ((Value<T>)initialValue).value;
            pointer.setter(pointer.evaluator(from, to, t));
        }

        public static void Interpolate<T>(T to, Pointer<T> pointer, float t)
        {
            if (to is not T) throw new InvalidOperationException("Argument 'to' is not the given type!");

            var from = pointer.getter();

            pointer.setter(pointer.evaluator(from, to, t));
        }
    }
}