using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class EventWrapper : MonoBehaviour
    {
        public BehaviourEngine engine { get; private set; }

        public void SetData(BehaviourEngine engine)
        {
            this.engine = engine;
        }

        private void Start()
        {
            engine.StartPlayback();
        }

        private void Update()
        {
            engine.Tick();
        }

        public void Select(RaycastHit hit, RaycastHit[] otherHits)
        {
            engine.Select(hit, otherHits);
        }
    }
}
