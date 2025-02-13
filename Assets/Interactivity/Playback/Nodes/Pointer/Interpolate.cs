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
            var d = 0.0f;

            int fromInt = 0;
            float fromFloat = 0.0f;
            Vector2 fromVec2 = Vector2.zero;
            Vector3 fromVec3 = Vector3.zero;
            Vector4 fromVec4 = Vector4.zero;
            Color fromColor = Color.white;
            Quaternion fromQuaternion = Quaternion.identity;
            Matrix4x4 fromMatrix = Matrix4x4.identity;

            switch (_interpGoal)
            {
                case Property<int>: // Cast to float so we can interp with int arguments.
                    fromInt = ((Pointer<int>)_pointer).getter();
                    break;
                case Property<float>:
                    fromFloat = ((Pointer<float>)_pointer).getter();
                    break;
                case Property<Vector2>:
                    fromVec2 = ((Pointer<Vector2>)_pointer).getter();
                    break;
                case Property<Vector3>:
                    {
                        switch(_pointer)
                        {
                            case Pointer<Color>:
                                fromColor = ((Pointer<Color>)_pointer).getter();
                                break;
                            case Pointer<Quaternion>:
                                fromQuaternion = ((Pointer<Quaternion>)_pointer).getter();
                                break;
                            case Pointer<Vector3>:
                                fromVec3 = ((Pointer<Vector3>)_pointer).getter();
                                break;
                        }
                    }
                    break;
                case Property<Vector4>:
                    {
                        switch(_pointer)
                        {
                            case Pointer<Color>:
                                fromColor = ((Pointer<Color>)_pointer).getter();
                                break;
                            case Pointer<Vector4>:
                                fromVec4 = ((Pointer<Vector4>)_pointer).getter();
                                break;
                        }
                    }
                    break;
                case Property<Matrix4x4>:
                    fromMatrix = ((Pointer<Matrix4x4>)_pointer).getter();
                    break;
                default:
                    throw new InvalidOperationException("No supported type found for interpolation.");
            }

            while(d <= 1.0f)
            {
                //Debug.Log($"INTERPOLATE {d}");
                if (IsCanceled())
                {
                    break;
                }

                if(IsPaused() == false)
                {
                // TODO: Replace these with the proper interpolate methods to match spec. Currently all linear interp.
                    switch (_interpGoal)
                    {
                        case Property<int> intProp: // Cast to float so we can interp with int arguments.

                            Helpers.Interpolate(fromInt, intProp.value, (Pointer<float>)_pointer, d);
                            break;
                        case Property<float> floatProp:
                            Helpers.Interpolate(fromFloat, floatProp.value, (Pointer<float>)_pointer, d);
                            break;
                        case Property<Vector2> vec2Prop:
                            Helpers.Interpolate(fromVec2, vec2Prop.value, (Pointer<Vector2>)_pointer, d);
                            break;
                        case Property<Vector3> vec3Prop:
                            {
                                switch (_pointer)
                                {
                                    case Pointer<Vector3>:
                                        Helpers.Interpolate(fromVec3, vec3Prop.value, (Pointer<Vector3>)_pointer, d);
                                        break;
                                    case Pointer<Color>:
                                        {
                                            Helpers.Interpolate(fromColor, vec3Prop.value.ToColor(), (Pointer<Color>)_pointer, d);
                                        }
                                        break;
                                    case Pointer<Quaternion>:
                                        {
                                            Helpers.Interpolate(fromQuaternion, Quaternion.Euler(vec3Prop.value), (Pointer<Quaternion>)_pointer, d);
                                        }
                                        break;
                                }           
                            }
                            break;
                        case Property<Vector4> vec4Prop:
                            {
                                switch(_pointer)
                                {
                                    case Pointer<Color>:
                                        Helpers.Interpolate(fromColor, vec4Prop.value, (Pointer<Color>)_pointer, d);
                                        break;
                                    case Pointer<Vector4>:
                                        Helpers.Interpolate(fromVec4, vec4Prop.value, (Pointer<Vector4>)_pointer, d);
                                        break;
                                }
                            }
                            break;
                        case Property<Matrix4x4> matrixProp:
                            Helpers.Interpolate(fromMatrix, matrixProp.value, (Pointer<Matrix4x4>)_pointer, d);
                            break;
                        default:
                            throw new InvalidOperationException("No supported type found for interpolation.");
                    }

                    d += Time.deltaTime / _duration;
                }
                await Task.Yield();
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