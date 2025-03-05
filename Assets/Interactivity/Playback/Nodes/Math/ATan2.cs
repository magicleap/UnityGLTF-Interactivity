using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathATan2 : BehaviourEngineNode
    {
        public MathATan2(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<float> aProp when b is Property<float> bProp => new Property<float>(math.atan2(aProp.value, bProp.value)),
                Property<Vector2> aProp when b is Property<Vector2> bProp => new Property<Vector2>(math.atan2(aProp.value, bProp.value)),
                Property<Vector2> aProp when b is Property<Vector2> bProp => new Property<Vector2>(math.atan2(aProp.value, bProp.value)),
                Property<Vector2> aProp when b is Property<Vector2> bProp => new Property<Vector2>(math.atan2(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}