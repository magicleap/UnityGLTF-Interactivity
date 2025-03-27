using System.Threading;
using UnityEngine;
using Unity.Mathematics;

namespace UnityGLTF.Interactivity
{
    public class DebugAssert : BehaviourEngineNode
    {
        public DebugAssert(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            Debug.Assert(a.GetSystemType() == b.GetSystemType(), $"The types don't match. Expected ({b.GetSystemType().ToString()}), got ({a.GetSystemType().ToString()})");

            if(a is Property<bool> aBool && b is Property<bool> bBool)
                Debug.Assert(aBool.value == bBool.value, $"The result({aBool.value}) is not matching the expected value({bBool.value}).");
            else if(a is Property<int> aInt && b is Property<int> bInt)
                Debug.Assert(aInt.value == bInt.value, $"The result({aInt.value}) is not matching the expected value({bInt.value}).");
            else if(a is Property<float> aFloat && b is Property<float> bFloat)
                Debug.Assert(EqualOrNan(aFloat.value, bFloat.value), $"The result({aFloat.value}) is not matching the expected value({bFloat.value}).");
            else if(a is Property<float2> aVec2 && b is Property<float2> bVec2)
                Debug.Assert(EqualOrNan(aVec2.value, bVec2.value), $"The result({aVec2.value}) is not matching the expected value({bVec2.value}).");
            else if(a is Property<float3> aVec3 && b is Property<float3> bVec3)
                Debug.Assert(EqualOrNan(aVec3.value, bVec3.value), $"The result({aVec3.value}) is not matching the expected value({bVec3.value}).");
            else if(a is Property<float4> aVec4 && b is Property<float4> bVec4)
                Debug.Assert(EqualOrNan(aVec4.value, bVec4.value), $"The result({aVec4.value}) is not matching the expected value({bVec4.value}).");

            TryExecuteFlow(ConstStrings.OUT);
        }

        private static bool EqualOrNan(float a, float b)
        {
            return (a == b) || (math.isnan(a) && math.isnan(b));
        }

        private static bool EqualOrNan(float2 a, float2 b)
        {
            return EqualOrNan(a.x, b.x) && EqualOrNan(a.y, b.y);
        }

        private static bool EqualOrNan(float3 a, float3 b)
        {
            return EqualOrNan(a.x, b.x) && EqualOrNan(a.y, b.y) && EqualOrNan(a.z, b.z);
        }

        private static bool EqualOrNan(float4 a, float4 b)
        {
            return EqualOrNan(a.x, b.x) && EqualOrNan(a.y, b.y) && EqualOrNan(a.z, b.z) && EqualOrNan(a.w, b.w);
        }
    }
}