using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathClamp : BehaviourEngineNode
    {
        public MathClamp(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);
            TryEvaluateValue(ConstStrings.C, out IProperty c);

            if (b is not Property<float> bProp)
                throw new InvalidOperationException($"B must be a float value.");

            if (c is not Property<float> cProp)
                throw new InvalidOperationException($"C must be a float value.");

            return a switch
            {
                Property<float> aProp => new Property<float>(math.clamp(aProp.value, bProp.value, cProp.value)),
                Property<float2> aProp => new Property<float2>(math.clamp(aProp.value, bProp.value, cProp.value)),
                Property<float3> aProp => new Property<float3>(math.clamp(aProp.value, bProp.value, cProp.value)),
                Property<float4> aProp => new Property<float4>(math.clamp(aProp.value, bProp.value, cProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}