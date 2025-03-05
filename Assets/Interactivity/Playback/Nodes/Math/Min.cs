using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathMin : BehaviourEngineNode
    {
        public MathMin(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<int> aProp when b is Property<int> bProp => new Property<int>(math.min(aProp.value, bProp.value)),
                Property<float> aProp when b is Property<float> bProp => new Property<float>(math.min(aProp.value, bProp.value)),
                Property<Vector2> aProp when b is Property<Vector2> bProp => new Property<Vector2>(math.min(aProp.value, bProp.value)),
                Property<Vector3> aProp when b is Property<Vector3> bProp => new Property<Vector3>(math.min(aProp.value, bProp.value)),
                Property<Vector4> aProp when b is Property<Vector4> bProp => new Property<Vector4>(math.min(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found or input types did not match."),
            };
        }
    }
}