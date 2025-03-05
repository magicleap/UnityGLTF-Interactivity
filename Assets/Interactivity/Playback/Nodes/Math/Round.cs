using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathRound : BehaviourEngineNode
    {
        public MathRound(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> prop => new Property<float>(math.round(prop.value)),
                Property<Vector2> prop => new Property<Vector2>(math.round(prop.value)),
                Property<Vector3> prop => new Property<Vector3>(math.round(prop.value)),
                Property<Vector4> prop => new Property<Vector4>(math.round(prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}