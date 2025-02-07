using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public struct NodePointers
    {
        public Pointer<Vector3> translation;
        public Pointer<Quaternion> rotation;
        public Pointer<Vector3> scale;
        public GameObject gameObject;

        public NodePointers(GameObject go)
        {
            gameObject = go;

            translation = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.position = v,
                getter = () => go.transform.position,
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            rotation = new Pointer<Quaternion>()
            {
                setter = (v) => go.transform.rotation = v,
                getter = () => go.transform.rotation,
                evaluator = (a, b, t) => Quaternion.Slerp(a, b, t)
            };

            scale = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localScale = v,
                getter = () => go.transform.localScale,
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };
        }
    }
}