using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathPow : BehaviourEngineNode
    {
        public MathPow(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<float> aFloat when b is Property<float> bFloat => new Property<float>(math.pow(aFloat.value, bFloat.value)),
                Property<float2> aVec2 when b is Property<float2> bVec2 => new Property<float2>(math.pow(aVec2.value, bVec2.value)),
                Property<float3> aVec3 when b is Property<float3> bVec3 => new Property<float3>(math.pow(aVec3.value, bVec3.value)),
                Property<float4> aVec4 when b is Property<float4> bVec4 => new Property<float4>(math.pow(aVec4.value, bVec4.value)),
                _ => throw new InvalidOperationException("No supported type found or input types did not match."),
            };
        }
    }
}