using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathATanH : BehaviourEngineNode
    {
        public MathATanH(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> floatProp => new Property<float>(ATanH(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(ATanH(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(ATanH(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(ATanH(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }

        private float ATanH(float x)
        {
            // 0.5 * ln((1+x)/(1-x))
            return 0.5f * math.log((1 + x) / (1 - x));
        }

        private Vector2 ATanH(float2 x)
        {
            return 0.5f * math.log((1 + x) / (1 - x));
        }

        private Vector3 ATanH(float3 x)
        {
            return 0.5f * math.log((1 + x) / (1 - x));
        }

        private Vector4 ATanH(float4 x)
        {
            return 0.5f * math.log((1 + x) / (1 - x));
        }
    }
}