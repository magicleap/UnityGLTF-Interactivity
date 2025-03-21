using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class DebugLog : BehaviourEngineNode
    {
        public DebugLog(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (!TryEvaluateValue("message", out IProperty message))
                TryExecuteFlow(ConstStrings.ERR);

            Util.Log(message.ToString());

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}