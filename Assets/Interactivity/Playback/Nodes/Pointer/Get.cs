using System;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class PointerGet : BehaviourEngineNode
    {
        public PointerGet(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        public override IProperty GetOutputValue(string id)
        {
            switch (id)
            {
                case "value":
                    return ResolvePointerValue();

                case "isValid":
                    return IsPointerValid();
            }

            throw new InvalidOperationException($"Socket {id} is not valid for this node!");
        }

        private Property<bool> IsPointerValid()
        {
            return new Property<bool>(TryGetPointerFromConfiguration(out IPointer pointer));
        }

        private IProperty ResolvePointerValue()
        {
            // TODO: Spec does not say what should happen here so we throw
            if (!TryGetPointerFromConfiguration(out IPointer pointer))
                throw new InvalidOperationException($"Cannot get value for invalid pointer.");

            return pointer switch
            {
                ReadOnlyPointer<int> pInt => new Property<int>(pInt.GetValue()),
                ReadOnlyPointer<float> pFloat => new Property<float>(pFloat.GetValue()),
                ReadOnlyPointer<Color> pColor => new Property<float4>(pColor.GetValue().ToFloat4()),
                ReadOnlyPointer<quaternion> pQuat => new Property<float4>(pQuat.GetValue().ToFloat4()),
                ReadOnlyPointer<float2> pVec2 => new Property<float2>(pVec2.GetValue()),
                ReadOnlyPointer<float3> pVec3 => new Property<float3>(pVec3.GetValue()),
                ReadOnlyPointer<float4> pVec4 => new Property<float4>(pVec4.GetValue()),
                Pointer<int> pInt => new Property<int>(pInt.GetValue()),
                Pointer<float> pFloat => new Property<float>(pFloat.GetValue()),
                Pointer<Color> pColor => new Property<float4>(pColor.GetValue().ToFloat4()),
                Pointer<quaternion> pQuat => new Property<float4>(pQuat.GetValue().ToFloat4()),
                Pointer<float2> pVec2 => new Property<float2>(pVec2.GetValue()),
                Pointer<float3> pVec3 => new Property<float3>(pVec3.GetValue()),
                Pointer<float4> pVec4 => new Property<float4>(pVec4.GetValue()),

                _ => throw new InvalidOperationException("No supported type found."),
            };
        }
    }
}