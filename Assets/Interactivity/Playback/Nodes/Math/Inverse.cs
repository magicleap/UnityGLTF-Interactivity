using System;
using Unity.Mathematics;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathInverse : BehaviourEngineNode
    {
        public MathInverse(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            bool isValid;

            var prop = a switch
            {
                // TODO: float2x2/3x3 support
                Property<Matrix4x4> aProp => new Property<Matrix4x4>(Inverse(aProp.value, out isValid)),
                _ => throw new InvalidOperationException("No supported type found."),
            };

            return id switch
            {
                ConstStrings.VALUE => prop,
                ConstStrings.IS_VALID => new Property<bool>(isValid),
                _ => throw new InvalidOperationException($"Requested output {id} is not part of the spec for this node."),
            };
        }

        private static Matrix4x4 Inverse (Matrix4x4 m, out bool isValid)
        {
            var inverse = Matrix4x4.Inverse(m);
            isValid = true;

            if (inverse == Matrix4x4.zero)
                isValid = false;

            return inverse;
        }
    }
}