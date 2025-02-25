using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public struct ActiveCameraPointers
    {
        public Pointer<Vector3> translation;
        public Pointer<Quaternion> rotation;

        public static ActiveCameraPointers CreatePointers()
        {
            // Unity coordinate system differs from the GLTF one.
            // Unity is left-handed with y-up and z-forward.
            // GLTF is right-handed with y-up and z-forward.
            // Handedness is easiest to swap here though we could do it during deserialization for performance.
            var pointers = new ActiveCameraPointers();

            pointers.translation = new Pointer<Vector3>()
            {
                setter = (v) => Camera.main.transform.localPosition = v.SwapHandedness(),
                getter = () => Camera.main.transform.localPosition.SwapHandedness(),
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            pointers.rotation = new Pointer<Quaternion>()
            {
                setter = (v) => Camera.main.transform.localRotation = v.SwapHandedness(),
                getter = () => Camera.main.transform.localRotation.SwapHandedness(),
                evaluator = (a, b, t) => Quaternion.Slerp(a, b, t)
            };

            return pointers;
        }
    }
}