using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathAbsSpec : NodeSpecifications
    {
        protected override (NodeFlow[] flows, NodeValue[] values) GenerateInputs()
        {
            var values = new NodeValue[]
            {
                new NodeValue(ConstStrings.A, "Argument.", new Type[]  { typeof(float), typeof(int), typeof(Vector2), typeof(Vector3), typeof(Vector4) }),
            };

            return (null, values);
        }

        protected override (NodeFlow[] flows, NodeValue[] values) GenerateOutputs()
        {
            var values = new NodeValue[]
            {
                new NodeValue(ConstStrings.VALUE, "If a > 0 then -a, else a.", new Type[]  { typeof(float), typeof(int), typeof(Vector2), typeof(Vector3), typeof(Vector4) }),
            };

            return (null, values);
        }
    }
}