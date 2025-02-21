using UnityEngine;

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

            translation = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localPosition = v,
                getter = () => go.transform.localPosition,
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            rotation = new Pointer<Quaternion>()
            {
                setter = (v) => go.transform.localRotation = v,
                getter = () => go.transform.localRotation,
                evaluator = (a, b, t) => Quaternion.Slerp(a, b, t)
            };

            scale = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localScale = v,
                getter = () => go.transform.localScale,
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