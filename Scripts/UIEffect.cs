using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// UIEffect.
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UIEffects/UIEffect", 1)]
    public class UIEffect : BaseMaterialEffect
    {
        static readonly ParameterTexture s_ParamTex = new(4, 128, "_ParamTex");

        [Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")] [SerializeField] [Range(0, 1)]
        float m_ColorFactor = 1;

        [Tooltip("Color effect mode")] [SerializeField]
        ColorMode m_ColorMode = ColorMode.Fill;

        /// <summary>
        /// Color effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float colorFactor
        {
            get => m_ColorFactor;
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_ColorFactor, value)) return;
                m_ColorFactor = value;
                SetEffectParamsDirty();
            }
        }

        /// <summary>
        /// Color effect mode.
        /// </summary>
        public ColorMode colorMode
        {
            get => m_ColorMode;
            set
            {
                if (m_ColorMode == value) return;
                m_ColorMode = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// Gets the parameter texture.
        /// </summary>
        public override ParameterTexture paramTex => s_ParamTex;

        protected override ulong GetMaterialHash(Material baseMaterial)
        {
            return MaterialCache.GetMaterialHash(baseMaterial, (int) m_ColorMode);
        }

        protected override Material CreateMaterial(Material baseMaterial)
        {
            var newShader = ShaderRepo.GetEffect(baseMaterial.shader.name);
            if (newShader is null) return null;

            var material = new Material(baseMaterial)
            {
                shader = newShader,
                hideFlags = HideFlags.HideAndDontSave
            };
            material.EnableKeyword(colorMode.GetKeyword());
            paramTex.RegisterToMaterial(material);
            return material;
        }

        /// <summary>
        /// Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(MeshBuilder mb)
        {
            var uvs = mb.UVs.Edit();
            int count = mb.UVs.Count;
            var normalizedIndex = paramTex.GetNormalizedIndex(this);

            for (var i = 0; i < count; i++)
            {
                var uv = uvs[i];
                uvs[i] = new Vector2(
                    Packer.ToFloat((uv.x + 0.5f) / 2f, (uv.y + 0.5f) / 2f),
                    normalizedIndex);
            }
        }

        protected override void SetEffectParamsDirty()
        {
            paramTex.SetData(this, 1, m_ColorFactor); // param.y : color factor
        }
    }
}