using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathXor : BehaviourEngineNode
    {
        public MathXor(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<int> aProp when b is Property<int> bProp => new Property<int>(aProp.value ^ bProp.value),
                Property<bool> aProp when b is Property<bool> bProp => new Property<bool>(aProp.value ^ bProp.value),
                _ => throw new InvalidOperationException("No supported type found or input types did not match."),
            };
        }
    }
}