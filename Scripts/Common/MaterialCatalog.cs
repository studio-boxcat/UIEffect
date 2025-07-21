using UnityEngine;
using UnityEngine.Assertions;

namespace Coffee.UIEffects
{
    static class MaterialCatalog
    {
        static Material _effect_Add;
        static Material _effect_Fill;
        static Material _shiny;
        static Material _shiny_PremultAlpha;


        static ParameterTexture _paramEffect;
        public static ParameterTexture ParamEffect => _paramEffect ??= CreateParamTexture(4, 32);
        static ParameterTexture _paramShiny;
        public static ParameterTexture ParamShiny => _paramShiny ??= CreateParamTexture(8, 32);

        static int _propertyIdCache;
        static int _propertyId
        {
            get
            {
                if (_propertyIdCache == 0)
                    _propertyIdCache = Shader.PropertyToID("_ParamTex");
                return _propertyIdCache;
            }
        }

        public static Material GetEffect(string baseShaderName, ColorMode colorMode)
        {
#if DEBUG
            Assert.IsTrue(IsValidShaderName(baseShaderName, colorMode),
                $"Invalid shader name: {baseShaderName}");
#endif

            return colorMode switch
            {
                ColorMode.Add => _effect_Add ??= LoadMaterial("UIEffect_ADD", ParamEffect),
                ColorMode.Fill => _effect_Fill ??= LoadMaterial("UIEffect_FILL", ParamEffect),
                _ => throw new System.NotSupportedException("Only ColorMode.Add and ColorMode.Fill are supported.")
            };
        }

#if DEBUG
        internal static bool IsValidShaderName(string shaderName, ColorMode colorMode)
        {
            if (colorMode is ColorMode.Add) // for add, if the shader is premult, we need to use blend function ONE+ONE, which is not implemented yet.
                return shaderName is "UI/Default";
            if (colorMode is ColorMode.Fill) // for fill, it does not matter whether it is premultiplied or not. the only alpha channel is used.
                return shaderName is "UI/Default" or "MeowTower/UI/UI-PremultAlpha";
            throw new System.NotSupportedException($"Unsupported ColorMode: {colorMode}");
        }
#endif

        public static Material GetShiny(string baseShaderName)
        {
            Assert.IsTrue(baseShaderName is "UI/Default" or "MeowTower/UI/UI-PremultAlpha",
                "Only UI/Default and UI/PremultAlpha are supported.");

            return baseShaderName is "UI/Default"
                ? _shiny ??= LoadMaterial("UIShiny", ParamShiny)
                : _shiny_PremultAlpha ??= LoadMaterial("UIShiny-PremultAlpha", ParamShiny);
        }

        static Material LoadMaterial(string path, ParameterTexture paramTex)
        {
            L.I($"LoadMaterial: {path}");
            var mat = Resources.Load<Material>(path);
#if UNITY_EDITOR
            mat = new Material(mat) { hideFlags = HideFlags.HideAndDontSave };
#endif
            Assert.IsNotNull(mat, $"Material not found: {path}");
            paramTex.SetTextureForMaterial(mat, _propertyId);
            return mat;
        }

        static ParameterTexture CreateParamTexture(int channels, int instanceLimit)
        {
            var paramTex = new ParameterTexture(channels, instanceLimit);
            paramTex.Initialize();
            return paramTex;
        }
    }
}