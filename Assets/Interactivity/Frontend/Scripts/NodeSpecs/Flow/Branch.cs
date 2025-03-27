
namespace UnityGLTF.Interactivity
{
    public class FlowBranchSpecs : NodeSpecifications
    {
        protected override NodeConfiguration[] GenerateConfiguration()
        {
            return new NodeConfiguration[]{
                new NodeConfiguration(ConstStrings.CONDITION, "Condition to evaluate flow path with.", typeof(bool)),
            };
        }

        protected override (NodeFlow[] flows, NodeValue[] values) GenerateInputs()
        {
            var flows = new NodeFlow[]{
                new NodeFlow(ConstStrings.IN, "The in flow.")
        };

            var values = new NodeValue[]{
            new NodeValue(ConstStrings.CONDITION, "Condition to evaluate.", new System.Type[] { typeof(bool) })
        };
            return (flows, values);
        }
    }
}