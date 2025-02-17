using System;

namespace UnityGLTF.Interactivity
{
    public interface IPointer
    {
    }

    public interface IPointer<T> : IPointer
    {
        public T GetValue();
    }
    public interface IReadOnlyPointer : IPointer
    {
    }

    public interface IReadOnlyPointer<T> : IReadOnlyPointer
    {
    }

    public struct ReadOnlyPointer<T> : IReadOnlyPointer<T>
    {
        public Func<T> getter;

        public T GetValue()
        {
            return getter();
        }

        public static implicit operator ReadOnlyPointer<T>(Pointer<T> pointer)
        {
            return new ReadOnlyPointer<T>() { getter = pointer.getter };
        }
    }

    public struct Pointer<T> : IPointer<T>
    {
        public Action<T> setter;
        public Func<T> getter;
        public Func<T, T, float, T> evaluator;

        public T GetValue()
        {
            return getter();
        }
    }

    public interface IValue
    {

    }

    public struct Value<T> : IValue
    {
        public T value;
    }
}