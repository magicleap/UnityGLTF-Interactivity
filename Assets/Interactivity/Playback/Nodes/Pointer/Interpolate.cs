using System;
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

        private IValue _initialValue; // container for initial value from _pointer

        public PointerInterpolate(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override async void Execute(string socket, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            TryExecuteFlow(ConstStrings.OUT);

            StartLongOperation();

            await SafeInterpolateAsync();

            EndLongOperation();
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

        private async Task InterpolateAsync()
        {
            // TODO: Replace these with the proper interpolate methods to match spec. Currently all linear interp.
            switch (_interpGoal)
            {
                case Property<int> intProp: // Cast to float so we can interp with int arguments.
                    await Helpers.InterpolateAsync(intProp.value, (Pointer<float>)_pointer, _duration);
                    break;
                case Property<float> floatProp:
                    await Helpers.InterpolateAsync(floatProp.value, (Pointer<float>)_pointer, _duration);
                    break;
                case Property<Vector2> vec2Prop:
                    await Helpers.InterpolateAsync(vec2Prop.value, (Pointer<Vector2>)_pointer, _duration);
                    break;
                case Property<Vector3> vec3Prop:
                    await ProcessVector3Async(_pointer, vec3Prop.value, _duration);
                    break;
                case Property<Vector4> vec4Prop:
                    await ProcessVector4Async(_pointer, vec4Prop.value, _duration);
                    break;
                case Property<Matrix4x4> matrixProp:
                    await Helpers.InterpolateAsync(matrixProp.value, (Pointer<Matrix4x4>)_pointer, _duration);
                    break;
                default:
                    throw new InvalidOperationException("No supported type found for interpolation.");
            }
        }

        private async Task SafeInterpolateAsync()
        {
            var t = 0.0f;        
            _initialValue = null;
          
            while(t <= 1.0f)
            {
                if (IsCanceled())
                {
                    break;
                }

                if(IsPaused() == false)
                {
                    ExecuteStep(t);

                    t += Time.deltaTime / _duration;
                }
                await Task.Yield();
            }
        }

        protected void ExecuteStep(float t)
        {
            // TODO: Replace these with the proper interpolate methods to match spec. Currently all linear interp.
            switch (_interpGoal)
            {
                case Property<int> intProp: // Cast to float so we can interp with int arguments.
                    Helpers.Interpolate(ref _initialValue, intProp.value, (Pointer<float>)_pointer, t);
                    break;
                case Property<float> floatProp:
                    Helpers.Interpolate(ref _initialValue, floatProp.value, (Pointer<float>)_pointer, t);
                    break;
                case Property<Vector2> vec2Prop:
                    Helpers.Interpolate(ref _initialValue, vec2Prop.value, (Pointer<Vector2>)_pointer, t);
                    break;
                case Property<Vector3> vec3Prop:
                    {
                        switch (_pointer)
                        {
                            case Pointer<Vector3>:
                                Helpers.Interpolate(ref _initialValue, vec3Prop.value, (Pointer<Vector3>)_pointer, t);
                                break;
                            case Pointer<Color>:
                                Helpers.Interpolate(ref _initialValue, vec3Prop.value.ToColor(), (Pointer<Color>)_pointer, t);
                                break;
                            case Pointer<Quaternion>:
                                Helpers.Interpolate(ref _initialValue, Quaternion.Euler(vec3Prop.value), (Pointer<Quaternion>)_pointer, t);
                                break;
                        }           
                    }
                    break;
                case Property<Vector4> vec4Prop:
                    {
                        switch(_pointer)
                        {
                            case Pointer<Color>:
                                Helpers.Interpolate(ref _initialValue, vec4Prop.value, (Pointer<Color>)_pointer, t);
                                break;
                            case Pointer<Vector4>:
                                Helpers.Interpolate(ref _initialValue, vec4Prop.value, (Pointer<Vector4>)_pointer, t);
                                break;
                        }
                    }
                    break;
                case Property<Matrix4x4> matrixProp:
                    Helpers.Interpolate(ref _initialValue, matrixProp.value, (Pointer<Matrix4x4>)_pointer, t);
                    break;
                default:
                    throw new InvalidOperationException("No supported type found for interpolation.");
            }
        }

        private async Task ProcessVector4Async(IPointer pointer, Vector4 value, float duration)
        {
            switch (pointer)
            {
                case Pointer<Vector4>:
                    await Helpers.InterpolateAsync(value, (Pointer<Vector4>)pointer, duration);
                    break;

                case Pointer<Color>:
                    await Helpers.InterpolateAsync(value, (Pointer<Color>)pointer, duration);
                    break;
            }
        }

        private async Task ProcessVector3Async(IPointer pointer, Vector3 value, float duration)
        {
            switch (pointer)
            {
                case Pointer<Vector3>:
                    await Helpers.InterpolateAsync(value, (Pointer<Vector3>)pointer, duration);
                    break;
                case Pointer<Color>:
                    await Helpers.InterpolateAsync(value.ToColor(), (Pointer<Color>)pointer, duration);
                    break;
                case Pointer<Quaternion>:
                    await Helpers.InterpolateAsync(Quaternion.Euler(value), (Pointer<Quaternion>)pointer, duration);
                    break;
            }
        }
    }
}