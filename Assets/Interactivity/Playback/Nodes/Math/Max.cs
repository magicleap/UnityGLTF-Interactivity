using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathMax : BehaviourEngineNode
    {
        public MathMax(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<int> aProp when b is Property<int> bProp => new Property<int>(math.max(aProp.value, bProp.value)),
                Property<float> aProp when b is Property<float> bProp => new Property<float>(math.max(aProp.value, bProp.value)),
                Property<float2> aProp when b is Property<float2> bProp => new Property<float2>(math.max(aProp.value, bProp.value)),
                Property<float3> aProp when b is Property<float3> bProp => new Property<float3>(math.max(aProp.value, bProp.value)),
                Property<float4> aProp when b is Property<float4> bProp => new Property<float4>(math.max(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found or input types did not match."),
            };
        }
    }
}