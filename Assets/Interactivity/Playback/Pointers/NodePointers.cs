using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public struct NodePointers
    {
        public Pointer<Vector3> translation;
        public Pointer<Quaternion> rotation;
        public Pointer<Vector3> scale;
        public Pointer<bool> visibility;
        public GameObject gameObject;

        public NodePointers(GameObject go)
        {
            gameObject = go;

            // Unity coordinate system differs from the GLTF one.
            // Unity is left-handed with y-up and z-forward.
            // GLTF is right-handed with y-up and z-forward.
            // Handedness is easiest to swap here though we could do it during deserialization for performance.

            translation = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localPosition = v.SwapHandedness(),
                getter = () => go.transform.localPosition.SwapHandedness(),
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            rotation = new Pointer<Quaternion>()
            {
                setter = (v) => go.transform.localRotation = v.SwapHandedness(),
                getter = () => go.transform.localRotation.SwapHandedness(),
                evaluator = (a, b, t) => Quaternion.Slerp(a, b, t)
            };

            scale = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localScale = v.SwapHandedness(),
                getter = () => go.transform.localScale.SwapHandedness(),
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            // TODO: Handle visibility/selectability pointers using extensions?
            // Need guidance on how we should handle these.
            visibility = new Pointer<bool>()
            {
                setter = (v) => go.SetActive(v),
                getter = () => go.activeSelf,
                evaluator = null
            };
        }
    }
}