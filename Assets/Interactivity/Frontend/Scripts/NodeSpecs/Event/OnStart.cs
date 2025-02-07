namespace UnityGLTF.Interactivity
{
    public class EventOnStartSpecs : NodeSpecifications
    {
        protected override (NodeFlow[] flows, NodeValue[] values) GenerateOutputs()
        {
            var flows = new NodeFlow[]
            {
                new NodeFlow(ConstStrings.OUT, "The flow to trigger when the session starts.")
            };

            return (flows, null);
        }
    }
}