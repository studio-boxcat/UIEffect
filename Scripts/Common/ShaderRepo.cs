using System;
using UnityEngine;

namespace Coffee.UIEffects
{
    public static class ShaderRepo
    {
        [NonSerialized] static Shader _effect;
        [NonSerialized] static Shader _shiny;
        [NonSerialized] static Shader _shiny_PremultAlpha;


        public static Shader GetEffect(string baseShaderName)
        {
            return baseShaderName switch
            {
                "UI/Default" => _effect ??= Resources.Load<Shader>("UIEffect"),
                _ => null
            };
        }

        public static Shader GetShiny(string baseShaderName)
        {
            return baseShaderName switch
            {
                "UI/Default" => _shiny ??= Resources.Load<Shader>("UIShiny"),
                "UI/PremultipliedAlpha" => _shiny_PremultAlpha ??= Resources.Load<Shader>("UIShiny-PremultAlpha"),
                _ => null
            };
        }
    }
}