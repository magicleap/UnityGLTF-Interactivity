using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class MathNaN : BehaviourEngineNode
    {
        public MathNaN(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            return new Property<float>(math.NAN);
        }
    }
}