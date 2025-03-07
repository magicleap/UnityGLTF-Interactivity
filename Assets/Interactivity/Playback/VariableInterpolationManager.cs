using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public struct VariableInterpolateData
    {
        public Variable variable;
        public float startTime;
        public float duration;
        public IProperty endValue;
        public Vector2 cp1;
        public Vector2 cp2;
        public Action done;
        public IInterpolator interpolator;
        public bool slerp;
    }

    public class VariableInterpolationManager
    {
        private struct Interpolator<T> : IInterpolator
        {
            public Variable variable;
            public Func<T, T, float, Property<T>> evaluator;
            public T from;
            public T to;

            public bool Interpolate(float t)
            {
                var end = t >= 1f;

                t = end ? 1f : t;

                variable.property = evaluator(from, to, t);

                return end;
            }
        }

        private readonly Dictionary<Variable, VariableInterpolateData> _interpolationsInProgress = new();

        public void OnTick()
        {
            // Avoiding iterating over a changing collection by grabbing a pooled dictionary.
            var temp = DictionaryPool<Variable, VariableInterpolateData>.Get();
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
                DictionaryPool<Variable, VariableInterpolateData>.Release(temp);
            }
        }

        private void DoInterpolate(VariableInterpolateData data)
        {
            var t = (Time.time - data.startTime) / data.duration;

            var finished = data.interpolator.Interpolate(t);

            if (finished)
            {
                Util.Log($"Finished Variable interpolate.");

                _interpolationsInProgress.Remove(data.variable);
                data.done();
            }
        }

        public void StartInterpolation(ref VariableInterpolateData data)
        {
            _interpolationsInProgress.Remove(data.variable); // Stop any in-progress interpolations for this pointer.

            var interpolator = GetInterpolator(data);

            data.interpolator = interpolator;

            _interpolationsInProgress.Add(data.variable, data);

            Util.Log($"Starting Variable Interpolation: Start Time {data.startTime}, Duration: {data.duration}");
        }

        private IInterpolator GetInterpolator(in VariableInterpolateData data)
        {
            var cp1 = data.cp1;
            var cp2 = data.cp2;

            var interpolator = data.variable.property switch
            {
                Property<float> => GetInterpolator(GetFloatEvaluator(cp1,cp2),  data),
                Property<Vector2> => GetInterpolator(GetVector2Evaluator(cp1, cp2), data),
                Property<Vector3> => GetInterpolator(GetVector3Evaluator(cp1, cp2), data),
                Property<Vector4> when data.slerp => GetInterpolator(GetQuaternionEvaluator(cp1, cp2), data),
                Property<Vector4> when !data.slerp=> GetInterpolator(GetVector4Evaluator(cp1, cp2), data),

                _ => throw new NotImplementedException(),
            };

            return interpolator;
        }

        private IInterpolator GetInterpolator<T>(Func<T, T, float, Property<T>> evaluator, in VariableInterpolateData data)
        {
            var variable = data.variable;
            var endValue = (Property<T>)data.endValue;

            return new Interpolator<T>()
            {
                variable = data.variable,
                evaluator = evaluator,
                from = ((Property<T>)variable.property).value,
                to = endValue.value
            };
        }

        private Func<float,float,float, Property<float>> GetFloatEvaluator(Vector2 cp1, Vector2 cp2)
        {
            return (a, b, t) => new Property<float>(Mathf.Lerp(a, b, Helpers.CubicBezier(t, cp1, cp2).y));
        }

        private Func<Vector2, Vector2, float, Property<Vector2>> GetVector2Evaluator(Vector2 cp1, Vector2 cp2)
        {
            return (a, b, t) => new Property<Vector2>(Vector2.Lerp(a, b, Helpers.CubicBezier(t, cp1, cp2).y));
        }

        private Func<Vector3, Vector3, float, Property<Vector3>> GetVector3Evaluator(Vector2 cp1, Vector2 cp2)
        {
            return (a, b, t) => new Property<Vector3>(Vector3.Lerp(a, b, Helpers.CubicBezier(t, cp1, cp2).y));
        }

        private Func<Vector4, Vector4, float, Property<Vector4>> GetVector4Evaluator(Vector2 cp1, Vector2 cp2)
        {
            return (a, b, t) => new Property<Vector4>(Vector4.Lerp(a, b, Helpers.CubicBezier(t, cp1, cp2).y));
        }

        private Func<Vector4, Vector4, float, Property<Vector4>> GetQuaternionEvaluator(Vector2 cp1, Vector2 cp2)
        {
            // Just a copy from unity mathematics library to avoid a bunch of type conversions.
            return (a, b, t) => new Property<Vector4>(Helpers.SlerpVector4(a, b, Helpers.CubicBezier(t, cp1, cp2).y));
        }
    }
}
