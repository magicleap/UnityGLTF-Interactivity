using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathCross : BehaviourEngineNode
    {
        public MathCross(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<Vector3> aProp when b is Property<Vector3> bProp => new Property<Vector3>(math.cross(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}