using UnityEngine;

namespace UnityGLTF.Interactivity.Frontend.Extensions
{
    public static class ExtensionMethods
    {
        public static void Reset(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }
}