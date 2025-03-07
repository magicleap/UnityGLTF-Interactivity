using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public struct NodePointers
    {
        public Pointer<Vector3> translation;
        public Pointer<Quaternion> rotation;
        public Pointer<Vector3> scale;
        public Pointer<bool> visibility;
        public Pointer<bool> selectability;
        public Pointer<bool> hoverability;
        public Pointer<float4x4> matrix;
        public Pointer<float4x4> globalMatrix;
        public GameObject gameObject;

        public NodePointers(GameObject go, GLTF.Schema.Node schema)
        {
            gameObject = go;

            // Unity coordinate system differs from the GLTF one.
            // Unity is left-handed with y-up and z-forward.
            // GLTF is right-handed with y-up and z-forward.
            // Handedness is easiest to swap here though we could do it during deserialization for performance.
            translation = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localPosition = v.SwapHandedness(),
                getter = () => go.transform.localPosition.SwapHandedness(),
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            rotation = new Pointer<Quaternion>()
            {
                setter = (v) => go.transform.localRotation = v.SwapHandedness(),
                getter = () => go.transform.localRotation.SwapHandedness(),
                evaluator = (a, b, t) => Quaternion.Slerp(a, b, t)
            };

            scale = new Pointer<Vector3>()
            {
                setter = (v) => go.transform.localScale = v.SwapHandedness(),
                getter = () => go.transform.localScale.SwapHandedness(),
                evaluator = (a, b, t) => Vector3.Lerp(a, b, t)
            };

            matrix = new Pointer<float4x4>()
            {
                setter = (v) => go.transform.SetFromLocalMatrix(v, isRightHanded: true),
                getter = () => go.transform.GetLocalMatrix(isRightHanded: true),
                evaluator = (a, b, t) => a.LerpToComponentwise(b, t) // Spec has floatNxN lerp componentwise.
            };

            globalMatrix = new Pointer<float4x4>()
            {
                setter = (v) => go.transform.SetFromWorldMatrix(v, isRightHanded: true),
                getter = () => go.transform.GetWorldMatrix(isRightHanded: true),
                evaluator = (a, b, t) => a.LerpToComponentwise(b, t) // Spec has floatNxN lerp componentwise.
            };

            // TODO: Handle visibility pointers better? Do we report the value back to the extension?
            // Should we make the extension handle the SetActive call so we just change the value of visibility?
            visibility = new Pointer<bool>()
            {
                setter = (v) => go.SetActive(v),
                getter = () => go.activeSelf,
                evaluator = null
            };

            selectability = GetSelectabilityPointers(schema);
            hoverability = GetHoverabilityPointers(schema);
        }

        private static Pointer<bool> GetSelectabilityPointers(GLTF.Schema.Node schema)
        {
            Pointer<bool> selectability;

            if (schema.Extensions != null && schema.Extensions.TryGetValue(GLTF.Schema.KHR_node_selectability_Factory.EXTENSION_NAME, out var extension))
            {
                var selectabilityExtension = extension as GLTF.Schema.KHR_node_selectability;

                selectability = new Pointer<bool>()
                {
                    setter = (v) => selectabilityExtension.selectable = v,
                    getter = () => selectabilityExtension.selectable,
                    evaluator = null
                };
            }
            else
            {
                selectability = new Pointer<bool>()
                {
                    setter = (v) => { },
                    getter = () => true,
                    evaluator = null
                };
            }

            return selectability;
        }

        private static Pointer<bool> GetHoverabilityPointers(GLTF.Schema.Node schema)
        {
            Pointer<bool> hoverability;

            if (schema.Extensions != null && schema.Extensions.TryGetValue(GLTF.Schema.KHR_node_hoverability_Factory.EXTENSION_NAME, out var extension))
            {
                var hoverabilityExtension = extension as GLTF.Schema.KHR_node_hoverability;

                hoverability = new Pointer<bool>()
                {
                    setter = (v) => hoverabilityExtension.hoverable = v,
                    getter = () => hoverabilityExtension.hoverable,
                    evaluator = null
                };
            }
            else
            {
                hoverability = new Pointer<bool>()
                {
                    setter = (v) => { },
                    getter = () => false,
                    evaluator = null
                };
            }

            return hoverability;
        }

        public static IPointer ProcessNodePointer(StringSpanReader reader, BehaviourEngineNode engineNode, List<NodePointers> pointers)
        {
            reader.AdvanceToNextToken('/');

            var nodeIndex = PointerResolver.GetNodeIndexFromArgument(reader, engineNode);

            var nodePointer = pointers[nodeIndex];

            reader.AdvanceToNextToken('/');

            // Path so far: /nodes/{}/
            return reader.AsReadOnlySpan() switch
            {
                var a when a.Is("translation") => nodePointer.translation,
                var a when a.Is("rotation") => nodePointer.rotation,
                var a when a.Is("scale") => nodePointer.scale,
                var a when a.Is("extensions") => ProcessExtensionPointer(reader, nodePointer),
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }

        private static IPointer ProcessExtensionPointer(StringSpanReader reader, NodePointers nodePointer)
        {
            reader.AdvanceToNextToken('/');

            // Path so far: /nodes/{}/extensions
            return reader.AsReadOnlySpan() switch
            {
                // TODO: Handle these properly via extensions in UnityGLTF?
                var a when a.Is("KHR_node_selectability") => nodePointer.selectability,
                var a when a.Is("KHR_node_visibility") => nodePointer.visibility,
                _ => throw new InvalidOperationException($"Extension {reader.ToString()} is unsupported at this time!"),
            };
        }
    }
}