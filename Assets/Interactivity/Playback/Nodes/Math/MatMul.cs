using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathMatMul : BehaviourEngineNode
    {
        public MathMatMul(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                // TODO: float2x2/3x3 support
                Property<Matrix4x4> aProp when b is Property<Matrix4x4> bProp => new Property<Matrix4x4>(math.mul(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}