using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathASinH : BehaviourEngineNode
    {
        public MathASinH(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> floatProp => new Property<float>(asinh(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(asinh(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(asinh(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(asinh(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }

        private float asinh(float x)
        {
            // ln(x + sqrt(x^2 + 1))
            return math.log(x + math.sqrt(x * x + 1));
        }

        private Vector2 asinh(float2 x)
        {
            return math.log(x + math.sqrt(x * x + 1));
        }

        private Vector3 asinh(float3 x)
        {
            return math.log(x + math.sqrt(x * x + 1));
        }

        private Vector4 asinh(float4 x)
        {
            return math.log(x + math.sqrt(x * x + 1));
        }
    }
}