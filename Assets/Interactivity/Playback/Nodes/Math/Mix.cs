using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathMix : BehaviourEngineNode
    {
        public MathMix(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);
            TryEvaluateValue(ConstStrings.C, out IProperty c);

            return a switch
            {
                Property<float> aProp when b is Property<float> bProp && c is Property<float> cProp => new Property<float>(math.lerp(aProp.value, bProp.value, cProp.value)),
                Property<Vector2> aProp when b is Property<Vector2> bProp && c is Property<Vector2> cProp => new Property<Vector2>(math.lerp(aProp.value, bProp.value, cProp.value)),
                Property<Vector3> aProp when b is Property<Vector3> bProp && c is Property<Vector3> cProp => new Property<Vector3>(math.lerp(aProp.value, bProp.value, cProp.value)),
                Property<Vector4> aProp when b is Property<Vector4> bProp && c is Property<Vector4> cProp => new Property<Vector4>(math.lerp(aProp.value, bProp.value, cProp.value)),
                _ => throw new InvalidOperationException("No supported type found or input types did not match."),
            };
        }
    }
}