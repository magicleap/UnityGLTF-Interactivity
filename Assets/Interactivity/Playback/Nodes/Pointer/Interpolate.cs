using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class PointerInterpolate : BehaviourEngineNode
    {
        private IPointer _pointer;
        private IProperty _interpGoal;
        private float _duration;

        public PointerInterpolate(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override async void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            if(validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            TryExecuteFlow(ConstStrings.OUT);

            // Only execute DONE flow if the interpolation was not cancelled.
            if(await InterpolateAsync(cancellationToken))
                TryExecuteFlow(ConstStrings.DONE);
        }

        public override bool ValidateConfiguration(string socket)
        {
            return TryGetPointerFromConfiguration(out _pointer) && 
                _pointer is not IReadOnlyPointer;
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.VALUE, out _interpGoal) && 
                TryEvaluateValue(ConstStrings.DURATION, out _duration);
        }

        private async Task<bool> InterpolateAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace these with the proper interpolate methods to match spec. Currently all linear interp.
            switch (_interpGoal)
            {
                case Property<int> intProp: // Cast to float so we can interp with int arguments.
                    return await Helpers.InterpolateAsync(intProp.value, (Pointer<float>)_pointer, _duration, cancellationToken);

                case Property<float> floatProp:
                    return await Helpers.InterpolateAsync(floatProp.value, (Pointer<float>)_pointer, _duration, cancellationToken);

                case Property<Vector2> vec2Prop:
                    return await Helpers.InterpolateAsync(vec2Prop.value, (Pointer<Vector2>)_pointer, _duration, cancellationToken);

                case Property<Vector3> vec3Prop:
                    return await ProcessVector3(_pointer, vec3Prop.value, _duration, cancellationToken);

                case Property<Vector4> vec4Prop:
                    return await ProcessVector4(_pointer, vec4Prop.value, _duration, cancellationToken);

                case Property<Matrix4x4> matrixProp:
                    return await Helpers.InterpolateAsync(matrixProp.value, (Pointer<Matrix4x4>)_pointer, _duration, cancellationToken);

                default:
                    throw new InvalidOperationException("No supported type found for interpolation.");
            }
        }

        private async Task<bool> ProcessVector4(IPointer pointer, Vector4 value, float duration, CancellationToken cancellationToken)
        {
            switch (pointer)
            {
                case Pointer<Vector4>:
                    return await Helpers.InterpolateAsync(value, (Pointer<Vector4>)pointer, duration, cancellationToken);

                case Pointer<Color>:
                    return await Helpers.InterpolateAsync(value, (Pointer<Color>)pointer, duration, cancellationToken);

                default:
                    throw new InvalidOperationException("No supported Pointer type for this Vector4 property.");
            }
        }

        private async Task<bool> ProcessVector3(IPointer pointer, Vector3 value, float duration, CancellationToken cancellationToken)
        {
            switch (pointer)
            {
                case Pointer<Vector3>:
                    return await Helpers.InterpolateAsync(value, (Pointer<Vector3>)pointer, duration, cancellationToken);

                case Pointer<Color>:
                    return await Helpers.InterpolateAsync(value.ToColor(), (Pointer<Color>)pointer, duration, cancellationToken);

                case Pointer<Quaternion>:
                    return await Helpers.InterpolateAsync(Quaternion.Euler(value), (Pointer<Quaternion>)pointer, duration, cancellationToken);

                default:
                    throw new InvalidOperationException("No supported Pointer type for this Vector3 property.");
            }
        }
    }
}