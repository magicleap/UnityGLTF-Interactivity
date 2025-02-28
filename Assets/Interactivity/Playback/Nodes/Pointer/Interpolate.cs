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
        private Vector2 _p1, _p2;

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
                TryEvaluateValue(ConstStrings.DURATION, out _duration) &&
                TryEvaluateValue(ConstStrings.P1, out _p1) &&
                TryEvaluateValue(ConstStrings.P2, out _p2);
        }

        private async Task<bool> InterpolateAsync(CancellationToken cancellationToken)
        {
            var data = new BezierInterpolateData()
            {
                pointer = _pointer,
                duration = _duration,
                cp0 = _p1,
                cp1 = _p2,
                cancellationToken = cancellationToken
            };

            return _interpGoal switch
            {
                // Cast to float so we can interp with int arguments.
                Property<int>       property => await Helpers.InterpolateBezierAsync(property, data),
                Property<float>     property => await Helpers.InterpolateBezierAsync(property, data),
                Property<Vector2>   property => await Helpers.InterpolateBezierAsync(property, data),
                Property<Vector3>   property => await ProcessVector3(property, data),
                Property<Vector4>   property => await ProcessVector4(property, data),
                Property<Matrix4x4> property => await Helpers.InterpolateBezierAsync(property, data),
                _ => throw new InvalidOperationException($"Type {_interpGoal.GetTypeSignature()} is not supported for interpolation."),
            };
        }

        private async Task<bool> ProcessVector4(Property<Vector4> property, BezierInterpolateData data)
        {
            return data.pointer switch
            {
                Pointer<Vector4> => await Helpers.InterpolateBezierAsync(property, data),
                Pointer<Color>   => await Helpers.InterpolateBezierAsync(property, data),
                _ => throw new InvalidOperationException($"Pointer type {data.pointer.GetSystemType()} is not supported for this Vector4 property."),
            };
        }

        private async Task<bool> ProcessVector3(Property<Vector3> property, BezierInterpolateData data)
        {
            return data.pointer switch
            {
                Pointer<Vector3>    => await Helpers.InterpolateBezierAsync(property, data),
                Pointer<Color>      => await Helpers.InterpolateBezierAsync(property.value.ToColor(), data),
                Pointer<Quaternion> => await Helpers.InterpolateBezierAsync(Quaternion.Euler(property.value), data),
                _ => throw new InvalidOperationException($"Pointer type {data.pointer.GetSystemType()} is not supported for this Vector3 property."),
            };
        }
    }
}