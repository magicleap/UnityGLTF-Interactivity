using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathExtract3 : BehaviourEngineNode
    {
        public MathExtract3(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            if (a is not Property<Vector3> property)
                throw new InvalidOperationException("Input A is not a Vector3!");

            switch (id)
            {
                case "0":
                    return new Property<float>(property.value.x);

                case "1":
                    return new Property<float>(property.value.y);

                case "2":
                    return new Property<float>(property.value.z);
            }

            throw new InvalidOperationException($"Socket {id} is not valid for this node!");
        }
    }
}