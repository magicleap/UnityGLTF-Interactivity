using System;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class MathMatDecompose : BehaviourEngineNode
    {
        public MathMatDecompose(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            if (a is not Property<float4x4> mProp)
                throw new InvalidOperationException($"Type of value a must be Matrix4x4 but a {a.GetTypeSignature()} was passed in!");

            // Conversion here since the Matrix4x4 methods all work and I'm too lazy to redo it for float4x4.
            var m = (Matrix4x4)mProp.value;
            m.GetTRS(out Vector3 translation, out Quaternion rotation, out Vector3 scale);

            return id switch
            {
                ConstStrings.TRANSLATION => new Property<float3>(translation),
                ConstStrings.ROTATION => new Property<float4>(rotation.ToFloat4()),
                ConstStrings.SCALE => new Property<float3>(scale),
                _ => throw new InvalidOperationException($"Requested output {id} is not part of the spec for this node."),
            };
        }
    }
}