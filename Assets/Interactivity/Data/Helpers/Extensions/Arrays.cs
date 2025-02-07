using UnityEngine;

namespace UnityGLTF.Interactivity.Extensions
{
    public static partial class ExtensionMethods
    {
        public static bool IsNullOrEmpty<T>(this T[] arr)
        {
            if (arr == null)
                return true;

            if (arr.Length <= 0)
                return true;

            return false;
        }
    }
}