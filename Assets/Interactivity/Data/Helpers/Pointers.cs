using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity.Materials
{
    public static class PointerHelpers
    {
        public static Pointer<T> CreatePointer<T>(Action<T> setter, Func<T> getter, Func<T, T, float, T> evaluator)
        {
            return new Pointer<T>()
            {
                setter = setter,
                getter = getter,
                evaluator = evaluator
            };
        }

        public static Pointer<float> CreateFloatPointer(Material mat, int hash)
        {
            return new Pointer<float>()
            {
                setter = (v) => mat.SetFloat(hash, v),
                getter = () => mat.GetFloat(hash),
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };
        }

        public static Pointer<Color> CreateColorPointer(Material mat, int hash)
        {
            return new Pointer<Color>()
            {
                setter = (v) => mat.SetColor(hash, v),
                getter = () => mat.GetColor(hash),
                evaluator = (a, b, t) => Color.Lerp(a, b, t)
            };
        }

        public static Pointer<Vector2> CreateOffsetPointer(Material mat, int hash)
        {
            return new Pointer<Vector2>()
            {
                setter = (v) => mat.SetTextureOffset(hash, v),
                getter = () => mat.GetTextureOffset(hash),
                evaluator = (a, b, t) => Vector2.Lerp(a, b, t)
            };
        }

        public static Pointer<Vector2> CreateScalePointer(Material mat, int hash)
        {
            return new Pointer<Vector2>()
            {
                setter = (v) => mat.SetTextureScale(hash, v),
                getter = () => mat.GetTextureScale(hash),
                evaluator = (a, b, t) => Vector2.Lerp(a, b, t)
            };
        }
    }
}