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

        public static IPointer ProcessMaterialPointer(string[] path, List<MaterialPointers> pointers)
        {
            var matIndex = int.Parse(path[2]);
            var matPointer = pointers[matIndex];
            var property = path[3];

            switch (property)
            {
                case "alphaCutoff":
                    return matPointer.alphaCutoff;

                case "emissiveFactor":
                    return matPointer.emissiveFactor;

                case "normalTexture":
                    return ProcessNormalMapPointer(path, matPointer);

                case "occlusionTexture":
                    return ProcessOcclusionMapPointer(path, matPointer);

                case "pbrMetallicRoughness":
                    return ProcessPBRMetallicRoughnessPointer(path, matPointer);
            }

            throw new InvalidOperationException("No valid property found for material.");
        }

        private static IPointer ProcessPBRMetallicRoughnessPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "baseColorFactor":
                    return matPointer.baseColorFactor;

                case "baseColorTexture":
                    return ProcessBaseColorTexturePointer(path, matPointer);

                case "metallicRoughnessTexture":
                    return ProcessMetallRoughnessTexturePointer(path, matPointer);

                case "metallicFactor":
                    return matPointer.metallicFactor;

                case "roughnessFactor":
                    return matPointer.roughnessFactor;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for PBR material.");
        }

        private static IPointer ProcessBaseColorTexturePointer(string[] path, MaterialPointers matPointer)
        {
            // TODO: These come in the form of baseColorTexture/extensions/KHR_texture_transform/{PROPERTY}
            // Don't skip the extensions/KHR_texture_transform bit.
            var subProperty = path[7];

            switch (subProperty)
            {
                case "offset":
                    return matPointer.baseOffset;

                case "rotation":
                    throw new NotImplementedException();

                case "scale":
                    return matPointer.baseScale;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for texture transform.");
        }

        private static IPointer ProcessMetallRoughnessTexturePointer(string[] path, MaterialPointers matPointer)
        {
            // TODO: These come in the form of metallicRoughnessTexture/extensions/KHR_texture_transform/{PROPERTY}
            // Don't skip the extensions/KHR_texture_transform bit.
            var subProperty = path[7];

            switch (subProperty)
            {
                case "offset":
                    return matPointer.metallicRoughnessOffset;

                case "rotation":
                    throw new NotImplementedException();

                case "scale":
                    return matPointer.metallicRoughnessScale;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for texture transform.");
        }

        private static IPointer ProcessOcclusionMapPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "strength":
                    return matPointer.occlusionTextureStrength;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for occlusion material.");
        }

        private static IPointer ProcessNormalMapPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "scale":
                    return matPointer.normalTextureScale;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for normal material.");
        }
    }
}