namespace UnityGLTF.Interactivity
{
    public class EventOnStart : BehaviourEngineNode
    {
        public EventOnStart(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onStart += OnStart;
        }

        private void OnStart()
        {
            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}