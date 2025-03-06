using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public struct InterpolateData
    {
        public IPointer pointer;
        public float startTime;
        public float duration;
        public IProperty endValue;
        public Vector2 cp1;
        public Vector2 cp2;
        public Action done;
        public IInterpolator interpolator;
    }

    public interface IInterpolator 
    {
        public bool Interpolate(float t);
    }
   
    public struct Interpolator<T> : IInterpolator
    {
        public Action<T> setter;
        public Func<T, T, float, T> evaluator;
        public T from;
        public T to;

        public bool Interpolate(float t)
        {
            var end = t >= 1f;

            t = end ? 1f : t;

            setter(evaluator(from, to, t));

            return end;
        }
    }

    public class InterpolationManager
    {
        private Dictionary<IPointer, InterpolateData> _interpolationsInProgress = new();

        public void OnTick()
        {
            // Avoiding iterating over a changing collection by grabbing a pooled dictionary.
            var temp = DictionaryPool<IPointer, InterpolateData>.Get();
            try
            {
                foreach (var interp in _interpolationsInProgress)
                {
                    temp.Add(interp.Key, interp.Value);
                }

                foreach (var anim in temp)
                {
                    DoInterpolate(anim.Value);
                }
            }
            finally
            {
                DictionaryPool<IPointer, InterpolateData>.Release(temp);
            }
        }

        private void DoInterpolate(InterpolateData data)
        {
            var t = (Time.time - data.startTime) / data.duration;

            var finished = data.interpolator.Interpolate(t);

            if (finished)
            {
                Util.Log($"Finished interpolating.");

                _interpolationsInProgress.Remove(data.pointer);
                data.done();
            }
        }

        public void StartInterpolation(ref InterpolateData data)
        {
            _interpolationsInProgress.Remove(data.pointer); // Stop any in-progress interpolations for this pointer.

            var interpolator = data.endValue switch
            {
                Property<float> property => GetInterpolator(property, data),
                Property<Vector2> property => GetInterpolator(property, data),
                Property<Vector3> property => ProcessVector3(property, data),
                Property<Vector4> property => ProcessVector4(property, data),
                Property<Matrix4x4> property => GetInterpolator(property, data),

                _ => throw new InvalidOperationException($"Type {data.endValue.GetTypeSignature()} is not supported for interpolation."),
            };

            data.interpolator = interpolator;

            _interpolationsInProgress.Add(data.pointer, data);

            Util.Log($"Starting Interpolation: Start Time {data.startTime}, Duration: {data.duration}");
        }

        private IInterpolator ProcessVector3(Property<Vector3> property, InterpolateData data)
        {
            return data.pointer switch
            {
                Pointer<Vector3> => GetInterpolator(property, data),
                Pointer<Color> => GetInterpolator(new Property<Color>(property.value.ToColor()), data),
                Pointer<Quaternion> => GetInterpolator(new Property<Quaternion>(Quaternion.Euler(property.value)), data),

                _ => throw new InvalidOperationException($"Pointer type {data.pointer.GetSystemType()} is not supported for this Vector3 property."),
            };
        }

        private IInterpolator ProcessVector4(Property<Vector4> property, InterpolateData data)
        {
            return data.pointer switch
            {
                Pointer<Vector4> => GetInterpolator(property, data),
                Pointer<Color> => GetInterpolator(new Property<Color>(property.value.ToColor()), data),
                Pointer<Quaternion> => GetInterpolator(new Property<Quaternion>(property.value.ToQuaternion()), data),

                _ => throw new InvalidOperationException($"Pointer type {data.pointer.GetSystemType()} is not supported for this Vector4 property."),
            };
        }

        private IInterpolator GetInterpolator<T>(Property<T> property, in InterpolateData data)
        {
            var p = (Pointer<T>)data.pointer;
            var cp1 = data.cp1;
            var cp2 = data.cp2;
            var evaluator = new Func<T, T, float, T>((a, b, t) => p.evaluator(a, b, Helpers.CubicBezier(t, cp1, cp2).y));

            var interpolator = new Interpolator<T>()
            {
                setter = p.setter,
                evaluator = evaluator,
                from = p.getter(),
                to = property.value
            };

            return interpolator;
        }
    }
}
