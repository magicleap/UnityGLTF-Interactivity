using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathNeg : BehaviourEngineNode
    {
        public MathNeg(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<int> intProp => new Property<int>(-intProp.value),
                Property<float> floatProp => new Property<float>(-floatProp.value),
                Property<Vector2> vector2Prop => new Property<Vector2>(-vector2Prop.value),
                Property<Vector3> vector3Prop => new Property<Vector3>(-vector3Prop.value),
                Property<Vector4> vector4Prop => new Property<Vector4>(-vector4Prop.value),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}