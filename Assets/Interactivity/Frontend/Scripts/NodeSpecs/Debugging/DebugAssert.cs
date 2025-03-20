using System;
using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class DebugAssertSpec : NodeSpecifications
    {
        protected override (NodeFlow[] flows, NodeValue[] values) GenerateInputs()
        {
            var values = new NodeValue[]
            {
                new NodeValue(ConstStrings.A, "Value to test.", new Type[]  { typeof(float), typeof(int), typeof(float2), typeof(float3), typeof(float4) }),
                new NodeValue(ConstStrings.B, "Value to test against.", new Type[]  { typeof(float), typeof(int), typeof(float2), typeof(float3), typeof(float4) }),
            };

            return (null, values);
        }
    }
}