using System;

namespace UnityGLTF.Interactivity
{
    public class VariableGet : BehaviourEngineNode
    {
        public VariableGet(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            if (!TryGetConfig(ConstStrings.VARIABLE, out int variableIndex))
                throw new InvalidOperationException();

            return engine.GetVariableProperty(variableIndex);
        }
    }
}