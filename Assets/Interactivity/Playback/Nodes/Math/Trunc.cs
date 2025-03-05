using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathTrunc : BehaviourEngineNode
    {
        public MathTrunc(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> prop => new Property<float>(math.trunc(prop.value)),
                Property<Vector2> prop => new Property<Vector2>(math.trunc(prop.value)),
                Property<Vector3> prop => new Property<Vector3>(math.trunc(prop.value)),
                Property<Vector4> prop => new Property<Vector4>(math.trunc(prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}