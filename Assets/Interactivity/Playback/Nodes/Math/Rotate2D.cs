using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathRotate2D : BehaviourEngineNode
    {
        public MathRotate2D(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);
            TryEvaluateValue(ConstStrings.B, out IProperty b);

            return a switch
            {
                Property<Vector2> aProp when b is Property<float> bProp => new Property<Vector2>(rotate(aProp.value, bProp.value)),
                _ => throw new InvalidOperationException("No supported type found."),
            };
        }

        private static Vector2 rotate(Vector2 v, float delta)
        {
            // TODO: Test rotation direction to make sure it matches the spec (counter-clockwise).
            return new Vector2(
                v.x * math.cos(delta) - v.y * math.sin(delta),
                v.x * math.sin(delta) + v.y * math.cos(delta)
            );
        }
    }
}