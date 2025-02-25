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
    }
}