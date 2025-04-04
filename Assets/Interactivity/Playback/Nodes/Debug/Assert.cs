using System.Threading;
using UnityEngine;
using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class DebugAssert : BehaviourEngineNode
    {
        private float _threshold = 0.0f;
        public DebugAssert(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty cond);

            string msg = "Assertion failed";
            if(TryEvaluateValue(ConstStrings.B, out IProperty b))
            {
                msg += $" ({b.ToString()})";
            }
            if(TryEvaluateValue(ConstStrings.C, out IProperty c))
            {
                msg += $" ({c.ToString()})";
            }

            Debug.Assert(((Property<bool>)cond).value, msg);

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}