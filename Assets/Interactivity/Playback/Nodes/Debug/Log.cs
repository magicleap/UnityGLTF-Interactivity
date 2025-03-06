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
            if (!TryEvaluateValue("message", out string message))
                TryExecuteFlow(ConstStrings.ERR);

            Util.Log(message);

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}