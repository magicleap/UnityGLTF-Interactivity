using System;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this ReadOnlySpan<char> span, string str)
        {
            return span.SequenceEqual(str.AsSpan());
        }
    }
}