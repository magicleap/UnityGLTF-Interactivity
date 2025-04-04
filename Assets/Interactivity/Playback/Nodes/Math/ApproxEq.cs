using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathApproxEq : MathSuperEq
    {
        private const float _threshold = 0.00001f;
        public MathApproxEq(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<bool> aProp when b is Property<bool> bProp => new Property<bool>(aProp.value == bProp.value),
                Property<int> aInt when b is Property<int> bInt => new Property<bool>(aInt.value == bInt.value),
                Property<float> aFloat when b is Property<float> bFloat => new Property<bool>(EqualOrNanOrInf(aFloat.value, bFloat.value)),
                Property<float2> pA when b is Property<float2> pB => new Property<bool>(EqualOrNanOrInf(pA.value, pB.value)),
                Property<float3> pA when b is Property<float3> pB => new Property<bool>(EqualOrNanOrInf(pA.value, pB.value)),
                Property<float4> pA when b is Property<float4> pB => new Property<bool>(EqualOrNanOrInf(pA.value, pB.value)),
                _ => throw new InvalidOperationException($"No supported type found or input types did not match. Types were A: {a.GetTypeSignature()}, B: {b.GetTypeSignature()}"),
            };
        }

        protected override bool EqualOrNanOrInf(float a, float b)
        {
            return (math.abs(a - b) <= _threshold) || (math.isnan(a) && math.isnan(b)) || (math.isinf(a) && math.isinf(b));
        }
    }
}