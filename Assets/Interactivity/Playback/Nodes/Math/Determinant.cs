using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathDeterminant : BehaviourEngineNode
    {
        public MathDeterminant(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                // TODO: float2x2/3x3 support
                Property<Matrix4x4> aProp => new Property<float>(math.determinant(aProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}