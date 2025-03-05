using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathATan : BehaviourEngineNode
    {
        public MathATan(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> floatProp => new Property<float>(math.atan(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(math.atan(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(math.atan(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(math.atan(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}