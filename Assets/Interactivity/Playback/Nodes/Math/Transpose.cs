using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathTranspose : BehaviourEngineNode
    {
        public MathTranspose(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                // TODO: float2x2/3x3 support
                Property<Matrix4x4> aProp => new Property<Matrix4x4>(math.transpose(aProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}