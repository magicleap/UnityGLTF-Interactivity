using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathMul : BehaviourEngineNode
    {
        public MathMul(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<int> aInt when b is Property<int> bInt => new Property<int>(aInt.value * bInt.value),
                Property<float> aFloat when b is Property<float> bFloat => new Property<float>(aFloat.value * bFloat.value),
                Property<Vector2> aVec2 when b is Property<Vector2> bVec2 => new Property<Vector2>(Vector2.Scale(aVec2.value, bVec2.value)),
                Property<Vector3> aVec3 when b is Property<Vector3> bVec3 => new Property<Vector3>(Vector3.Scale(aVec3.value, bVec3.value)),
                Property<Vector4> aVec4 when b is Property<Vector4> bVec4 => new Property<Vector4>(Vector4.Scale(aVec4.value, bVec4.value)),
                _ => throw new InvalidOperationException("No supported type found or input types did not match."),
            };
        }
    }
}