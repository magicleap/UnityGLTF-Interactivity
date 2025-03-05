using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathRotate3D : BehaviourEngineNode
    {
        public MathRotate3D(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);
            TryEvaluateValue(ConstStrings.C, out IProperty c);

            return a switch
            {
                Property<Vector3> aProp when b is Property<Vector3> bProp && c is Property<float> cProp => new Property<Vector3>(rotate(aProp.value, bProp.value, cProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }

        private static Vector3 rotate(float3 vec, Vector3 axis, float rad)
        {
            // TODO: Test rotation direction to make sure it matches the spec (counter-clockwise).
            return math.mul(quaternion.AxisAngle(axis, rad), vec);
        }
    }
}