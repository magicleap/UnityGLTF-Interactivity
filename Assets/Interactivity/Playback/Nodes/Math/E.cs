using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class MathE : BehaviourEngineNode
    {
        public MathE(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            return new Property<float>(math.E);
        }
    }
}