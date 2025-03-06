using System.Threading;

namespace UnityGLTF.Interactivity
{
    public class FlowSequence : BehaviourEngineNode
    {
        public FlowSequence(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            for (int i = 0; i < node.flows.Count; i++)
            {
                engine.ExecuteFlow(node.flows[i]);
            }
        }
    }
}