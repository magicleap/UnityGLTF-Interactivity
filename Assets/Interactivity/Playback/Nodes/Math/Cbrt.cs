using System;
using Unity.Mathematics;
using UnityEngine;


namespace UnityGLTF.Interactivity
{
    public class MathCbrt : BehaviourEngineNode
    {
        public MathCbrt(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            return a switch
            {
                Property<float> floatProp => new Property<float>(MathF.Cbrt(floatProp.value)),
                Property<Vector2> vector2Prop => new Property<Vector2>(Cbrt(vector2Prop.value)),
                Property<Vector3> vector3Prop => new Property<Vector3>(Cbrt(vector3Prop.value)),
                Property<Vector4> vector4Prop => new Property<Vector4>(Cbrt(vector4Prop.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }

        private static Vector2 Cbrt(Vector2 v)
        {
            return new Vector2(MathF.Cbrt(v.x), MathF.Cbrt(v.y));
        }

        private static Vector3 Cbrt(Vector3 v)
        {
            return new Vector3(MathF.Cbrt(v.x), MathF.Cbrt(v.y), MathF.Cbrt(v.z));
        }

        private static Vector4 Cbrt(Vector4 v)
        {
            return new Vector4(MathF.Cbrt(v.x), MathF.Cbrt(v.y), MathF.Cbrt(v.z), MathF.Cbrt(v.w));
        }
    }
}