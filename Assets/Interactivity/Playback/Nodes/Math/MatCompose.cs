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
                // TODO: float2x2/3x3 support
                Property<Vector3> tProp when rotation is Property<Vector4> rProp && scale is Property<Vector3> sProp => new Property<Matrix4x4>(Matrix4x4.TRS(tProp.value, rProp.value.ToQuaternion(), sProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}