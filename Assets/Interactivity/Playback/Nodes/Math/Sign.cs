using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathSign : BehaviourEngineNode
    {
        public MathSign(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<int> prop => new Property<int>((int)math.sign(prop.value)),
                Property<float> prop => new Property<float>(math.sign(prop.value)),
                Property<Vector2> prop => new Property<Vector2>(math.sign(prop.value)),
                Property<Vector3> prop => new Property<Vector3>(math.sign(prop.value)),
                Property<Vector4> prop => new Property<Vector4>(math.sign(prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}