using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathSin : BehaviourEngineNode
    {
        public MathSin(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> floatProp => new Property<float>(math.sin(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(math.sin(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(math.sin(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(math.sin(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}