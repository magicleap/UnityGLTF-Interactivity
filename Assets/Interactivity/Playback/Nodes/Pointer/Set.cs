using System;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class PointerSet : BehaviourEngineNode
    {
        private IPointer _pointer;
        private IProperty _property;

        public PointerSet(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            switch (_property)
            {
                case Property<int> prop when _pointer is Pointer<int> p:
                    p.setter(prop.value);
                    break;
                case Property<float> prop when _pointer is Pointer<float> p:
                    p.setter(prop.value);
                    break;
                case Property<Vector3> prop when _pointer is Pointer<Vector3> p:
                    p.setter(prop.value);
                    break;
                case Property<Vector3> prop when _pointer is Pointer<Color> p:
                    p.setter(prop.value.ToColor());
                    break;
                case Property<Vector3> prop when _pointer is Pointer<Quaternion> p:
                    p.setter(Quaternion.Euler(prop.value));
                    break;
                case Property<Vector4> prop when _pointer is Pointer<Vector4> p:
                    p.setter(prop.value);
                    break;
                case Property<Vector4> prop when _pointer is Pointer<Color> p:
                    p.setter(prop.value.ToColor());
                    break;
                default:
                    throw new InvalidOperationException("No supported type found for interpolation.");
            }

            TryExecuteFlow(ConstStrings.OUT);
        }

        public override bool ValidateConfiguration(string socket)
        {
            return TryGetPointerFromConfiguration(out _pointer) &&
                _pointer is not IReadOnlyPointer;
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.VALUE, out _property);
        }
    }
}