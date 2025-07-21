using Sirenix.OdinInspector;
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
#if UNITY_EDITOR
        , ISelfValidator
#endif
    {
        [Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")]
        [SerializeField] [Range(0, 1)]
        [OnValueChanged(nameof(SetEffectParamsDirty))]
        float m_ColorFactor = 1;

        [Tooltip("Color effect mode")]
        [SerializeField]
        [OnValueChanged(nameof(SetMaterialDirty))]
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
        public override ParameterTexture paramTex => MaterialCatalog.ParamEffect;

        protected override Material GetEffectMaterial(Material baseMaterial)
        {
            return MaterialCatalog.GetEffect(baseMaterial.shader.name, colorMode);
        }

        /// <summary>
        /// Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(MeshBuilder mb)
        {
            var uvs = mb.UVs.Edit();
            var count = uvs.Length;
            var normalizedIndex = paramTex.GetNormalizedIndex(this);

            for (var i = 0; i < count; i++)
            {
                var uv = uvs[i];
                uvs[i] = new Vector2(
                    Packer.Pack((uv.x + 0.5f) / 2f, (uv.y + 0.5f) / 2f),
                    normalizedIndex);
            }
        }

        protected override void SetEffectParamsDirty()
        {
            paramTex.SetData(this, 1, m_ColorFactor); // param.y : color factor
        }

#if UNITY_EDITOR
        void ISelfValidator.Validate(SelfValidationResult result)
        {
            var shaderName = GetComponent<Graphic>().material.shader.name;
            if (!MaterialCatalog.IsValidShaderName(shaderName, colorMode))
                result.AddError($"The shader '{shaderName}' is not a valid UIEffect shader.");
        }
#endif
    }
}