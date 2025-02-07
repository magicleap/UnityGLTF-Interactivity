namespace UnityGLTF.Interactivity
{
    public interface IProperty
    {
        public string ToString();
    }

    public struct Property<T> : IProperty
    {
        public Property(T value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public T value;
    }
}