using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathLength : BehaviourEngineNode
    {
        public MathLength(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> aProp => new Property<float>(math.length(aProp.value)),
                Property<Vector2> aProp => new Property<float>(math.length(aProp.value)),
                Property<Vector3> aProp => new Property<float>(math.length(aProp.value)),
                Property<Vector4> aProp => new Property<float>(math.length(aProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}