using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathTransform : BehaviourEngineNode
    {
        public MathTransform(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                // TODO: float2x2/3x3 support
                Property<Vector4> aProp when b is Property<Matrix4x4> bProp => new Property<Vector4>(math.mul(bProp.value, aProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}