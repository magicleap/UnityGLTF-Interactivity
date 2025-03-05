using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathCeil : BehaviourEngineNode
    {
        public MathCeil(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> aProp => new Property<float>(math.ceil(aProp.value)),
                Property<Vector2> aProp => new Property<Vector2>(math.ceil(aProp.value)),
                Property<Vector3> aProp => new Property<Vector3>(math.ceil(aProp.value)),
                Property<Vector4> aProp => new Property<Vector4>(math.ceil(aProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}