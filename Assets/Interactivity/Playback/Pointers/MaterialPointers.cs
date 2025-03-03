using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public struct MaterialPointers
    {
        private static readonly int baseColorTexture = Shader.PropertyToID("baseColorTexture");
        private static readonly int metallicRoughnessTexture = Shader.PropertyToID("metallicRoughnessTexture");
        private static readonly int baseColorFactorHash = Shader.PropertyToID("baseColorFactor");
        private static readonly int metallicFactorHash = Shader.PropertyToID("metallicFactor");
        private static readonly int roughnessFactorHash = Shader.PropertyToID("roughnessFactor");
        private static readonly int normalScaleHash = Shader.PropertyToID("normalScale");
        private static readonly int occlusionStrengthHash = Shader.PropertyToID("occlusionStrength");
        private static readonly int emissiveFactorHash = Shader.PropertyToID("emissiveFactor");
        private static readonly int alphaCutoffHash = Shader.PropertyToID("alphaCutoff");

        public Pointer<float> alphaCutoff;
        public Pointer<Color> emissiveFactor;
        public Pointer<float> normalTextureScale;
        public Pointer<float> occlusionTextureStrength;
        public Pointer<Color> baseColorFactor;
        public Pointer<float> metallicFactor;
        public Pointer<float> roughnessFactor;

        public Pointer<Vector2> baseOffset;
        public Pointer<Vector2> baseScale;

        public Pointer<Vector2> metallicRoughnessOffset;
        public Pointer<Vector2> metallicRoughnessScale;

        public MaterialPointers(Material mat)
        {
            // Textures
            baseOffset = CreateOffsetPointer(mat, baseColorTexture);
            baseScale = CreateScalePointer(mat, baseColorTexture);

            metallicRoughnessOffset = CreateOffsetPointer(mat, metallicRoughnessTexture);
            metallicRoughnessScale = CreateScalePointer(mat, metallicRoughnessTexture);

            // Colors
            emissiveFactor = CreateColorPointer(mat, emissiveFactorHash);
            baseColorFactor = CreateColorPointer(mat, baseColorFactorHash);

            // Floats
            alphaCutoff = CreateFloatPointer(mat, alphaCutoffHash);
            normalTextureScale = CreateFloatPointer(mat, normalScaleHash);
            occlusionTextureStrength = CreateFloatPointer(mat, occlusionStrengthHash);
            metallicFactor = CreateFloatPointer(mat, metallicFactorHash);
            roughnessFactor = CreateFloatPointer(mat, roughnessFactorHash);

            Pointer<float> CreateFloatPointer(Material mat, int hash)
            {
                return new Pointer<float>()
                {
                    setter = (v) => mat.SetFloat(hash, v),
                    getter = () => mat.GetFloat(hash),
                    evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
                };
            }

            Pointer<Color> CreateColorPointer(Material mat, int hash)
            {
                return new Pointer<Color>()
                {
                    setter = (v) => mat.SetColor(hash, v),
                    getter = () => mat.GetColor(hash),
                    evaluator = (a, b, t) => Color.Lerp(a, b, t)
                };
            }

            Pointer<Vector2> CreateOffsetPointer(Material mat, int hash)
            {
                return new Pointer<Vector2>()
                {
                    setter = (v) => mat.SetTextureOffset(hash, v),
                    getter = () => mat.GetTextureOffset(hash),
                    evaluator = (a, b, t) => Vector2.Lerp(a, b, t)
                };
            }

            Pointer<Vector2> CreateScalePointer(Material mat, int hash)
            {
                return new Pointer<Vector2>()
                {
                    setter = (v) => mat.SetTextureScale(hash, v),
                    getter = () => mat.GetTextureScale(hash),
                    evaluator = (a, b, t) => Vector2.Lerp(a, b, t)
                };
            }
        }

        public static IPointer ProcessMaterialPointer(StringSpanReader reader, BehaviourEngineNode engineNode, List<MaterialPointers> pointers)
        {
            reader.AdvanceToNextToken('/');

            var nodeIndex = PointerResolver.GetNodeIndexFromArgument(reader, engineNode);

            var pointer = pointers[nodeIndex];

            reader.AdvanceToNextToken('/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("alphaCutoff".AsSpan()) => pointer.alphaCutoff,
                var a when a.SequenceEqual("emissiveFactor".AsSpan()) => pointer.emissiveFactor,
                var a when a.SequenceEqual("normalTexture".AsSpan()) => ProcessNormalMapPointer(reader, pointer),
                var a when a.SequenceEqual("occlusionTexture".AsSpan()) => ProcessOcclusionMapPointer(reader, pointer),
                var a when a.SequenceEqual("pbrMetallicRoughness".AsSpan()) => ProcessPBRMetallicRoughnessPointer(reader, pointer),
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }

        private static IPointer ProcessPBRMetallicRoughnessPointer(StringSpanReader reader, MaterialPointers matPointer)
        {
            reader.AdvanceToNextToken('/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("baseColorFactor".AsSpan()) => matPointer.baseColorFactor,
                var a when a.SequenceEqual("baseColorTexture".AsSpan()) => ProcessBaseColorTexturePointer(reader, matPointer),
                var a when a.SequenceEqual("metallicRoughnessTexture".AsSpan()) => ProcessMetallRoughnessTexturePointer(reader, matPointer),
                var a when a.SequenceEqual("metallicFactor".AsSpan()) => matPointer.metallicFactor,
                var a when a.SequenceEqual("roughnessFactor".AsSpan()) => matPointer.roughnessFactor,
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }

        private static IPointer ProcessBaseColorTexturePointer(StringSpanReader reader, MaterialPointers matPointer)
        {
            // TODO: These come in the form of baseColorTexture/extensions/KHR_texture_transform/{PROPERTY}
            // We're skipping ahead to get there with this triple-call.
            reader.AdvanceToNextToken('/');
            reader.AdvanceToNextToken('/');
            reader.AdvanceToNextToken('/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("offset".AsSpan()) => matPointer.baseOffset,
                var a when a.SequenceEqual("rotation".AsSpan()) => throw new NotImplementedException(),
                var a when a.SequenceEqual("scale".AsSpan()) => matPointer.baseScale,
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }

        private static IPointer ProcessMetallRoughnessTexturePointer(StringSpanReader reader, MaterialPointers matPointer)
        {
            // TODO: These come in the form of baseColorTexture/extensions/KHR_texture_transform/{PROPERTY}
            // We're skipping ahead to get there with this triple-call.
            reader.AdvanceToNextToken('/');
            reader.AdvanceToNextToken('/');
            reader.AdvanceToNextToken('/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("offset".AsSpan()) => matPointer.metallicRoughnessOffset,
                var a when a.SequenceEqual("rotation".AsSpan()) => throw new NotImplementedException(),
                var a when a.SequenceEqual("scale".AsSpan()) => matPointer.metallicRoughnessScale,
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }

        private static IPointer ProcessOcclusionMapPointer(StringSpanReader reader, MaterialPointers matPointer)
        {
            reader.AdvanceToNextToken('/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("strength".AsSpan()) => matPointer.occlusionTextureStrength,
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }

        private static IPointer ProcessNormalMapPointer(StringSpanReader reader, MaterialPointers matPointer)
        {
            reader.AdvanceToNextToken('/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("scale".AsSpan()) => matPointer.normalTextureScale,
                _ => throw new InvalidOperationException($"Property {reader.ToString()} is unsupported at this time!"),
            };
        }
    }
}