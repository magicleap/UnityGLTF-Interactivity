using System;
using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class DebugLogSpec : NodeSpecifications
    {
        protected override (NodeFlow[] flows, NodeValue[] values) GenerateInputs()
        {
            var values = new NodeValue[]
            {
                new NodeValue("message", "Argument to print.", new Type[]  { typeof(float), typeof(int), typeof(float2), typeof(float3), typeof(float4) }),
            };

            return (null, values);
        }
    }
}