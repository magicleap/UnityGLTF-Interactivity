using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class MathInf : BehaviourEngineNode
    {
        public MathInf(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            return new Property<float>(math.INFINITY);
        }
    }
}