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

        private void OnDisable()
        {
            engine.StopPlayback();
        }

        private void Update()
        {
            engine.Tick();
        }

        public void Select(GameObject go)
        {
            engine.Select(go);
        }

        public void PauseResume()
        {
            if(engine.IsPaused())
                engine.Resume();
            else
                engine.Pause();
        }
    }
}
