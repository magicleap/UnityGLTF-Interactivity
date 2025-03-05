using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathLog10 : BehaviourEngineNode
    {
        public MathLog10(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> floatProp => new Property<float>(math.log10(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(math.log10(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(math.log10(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(math.log10(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}