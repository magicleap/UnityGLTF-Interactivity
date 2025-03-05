using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathDot : BehaviourEngineNode
    {
        public MathDot(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<Vector2> aProp when b is Property<Vector2> bProp => new Property<float>(math.dot(aProp.value, bProp.value)),
                Property<Vector3> aProp when b is Property<Vector3> bProp => new Property<float>(math.dot(aProp.value, bProp.value)),
                Property<Vector4> aProp when b is Property<Vector4> bProp => new Property<float>(math.dot(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}