using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathNormalize : BehaviourEngineNode
    {
        public MathNormalize(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<Vector2> aProp => new Property<Vector2>(math.normalize(aProp.value)),
                Property<Vector3> aProp => new Property<Vector3>(math.normalize(aProp.value)),
                Property<Vector4> aProp => new Property<Vector4>(math.normalize(aProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}