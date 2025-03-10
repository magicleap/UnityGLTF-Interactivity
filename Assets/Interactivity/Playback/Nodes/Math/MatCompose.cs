using System;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class MathMatCompose : BehaviourEngineNode
    {
        public MathMatCompose(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.TRANSLATION, out IProperty translation);
            TryEvaluateValue(ConstStrings.ROTATION, out IProperty rotation);
            TryEvaluateValue(ConstStrings.SCALE, out IProperty scale);

            return translation switch
            {
                Property<float3> tProp when rotation is Property<float4> rProp && scale is Property<float3> sProp => new Property<float4x4>(TRS(tProp.value, rProp.value.ToQuaternion(), sProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }

        /// <summary>
        /// Added because Unity's Mathematics library has a bug where float4x4.TRS creates a TSR matrix.
        /// </summary>
        public static float4x4 TRS(float3 translation, quaternion rotation, float3 scale)
        {
            float3x3 m = math.mul(Unity.Mathematics.float3x3.Scale(scale), new float3x3(rotation));
            return new float4x4(new float4(m.c0, 0.0f),
            new float4(m.c1, 0.0f),
            new float4(m.c2, 0.0f),
            new float4(translation, 1.0f));
        }
    }
}