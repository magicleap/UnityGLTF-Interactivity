using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class MathExtract4 : BehaviourEngineNode
    {
        public MathExtract4(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            TryEvaluateValue(ConstStrings.A, out IProperty a);

            if (a is not Property<Vector4> property)
                throw new InvalidOperationException("Input A is not a Vector4!");

            switch (id)
            {
                case "0":
                    return new Property<float>(property.value.x);

                case "1":
                    return new Property<float>(property.value.y);

                case "2":
                    return new Property<float>(property.value.z);

                case "3":
                    return new Property<float>(property.value.w);
            }

            throw new InvalidOperationException($"Socket {id} is not valid for this node!");
        }
    }
}