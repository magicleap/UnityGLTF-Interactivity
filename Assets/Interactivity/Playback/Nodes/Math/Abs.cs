using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathAbs : BehaviourEngineNode
    {
        public MathAbs(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<int> intProp => new Property<int>(math.abs(intProp.value)),
                Property<float> floatProp => new Property<float>(math.abs(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(math.abs(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(math.abs(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(math.abs(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}