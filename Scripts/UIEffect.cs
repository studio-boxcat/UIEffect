using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// UIEffect.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UIEffects/UIEffect", 1)]
    public class UIEffect : BaseMaterialEffect, IMaterialModifier
    {
        private const uint k_ShaderId = 2 << 3;
        private static readonly ParameterTexture s_ParamTex = new ParameterTexture(4, 1024, "_ParamTex");

        [Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")] [SerializeField] [Range(0, 1)]
        float m_ColorFactor = 1;

        [Tooltip("Color effect mode")] [SerializeField]
        ColorMode m_ColorMode = ColorMode.Multiply;

        /// <summary>
        /// Color effect factor between 0(no effect) and 1(complete effect).
        /// </summary>
        public float colorFactor
        {
            get { return m_ColorFactor; }
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
            get { return m_ColorMode; }
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
        public override ParameterTexture paramTex
        {
            get { return s_ParamTex; }
        }

        public override Hash128 GetMaterialHash(Material material)
        {
            if (!isActiveAndEnabled || !material || !material.shader)
                return k_InvalidHash;

            var shaderVariantId = (uint) ((int) m_ColorMode << 9);
            return new Hash128(
                (uint) material.GetInstanceID(),
                k_ShaderId + shaderVariantId,
                0,
                0
            );
        }

        public override void ModifyMaterial(Material newMaterial, Graphic graphic)
        {
            newMaterial.shader = Shader.Find(string.Format("Hidden/{0} (UIEffect)", newMaterial.shader.name));
            SetShaderVariants(newMaterial, m_ColorMode);

            paramTex.RegisterMaterial(newMaterial);
        }

        /// <summary>
        /// Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(VertexHelper vh, Graphic graphic)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var normalizedIndex = paramTex.GetNormalizedIndex(this);

            {
                int count = vh.currentVertCount;
                UIVertex vt = default(UIVertex);
                for (int i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref vt, i);
                    Vector2 uv0 = vt.uv0;
                    vt.uv0 = new Vector2(
                        Packer.ToFloat((uv0.x + 0.5f) / 2f, (uv0.y + 0.5f) / 2f),
                        normalizedIndex
                    );
                    vh.SetUIVertex(vt, i);
                }
            }
        }

        /// <summary>
        /// Modifies the mesh.
        /// </summary>
        public override void ModifyMesh(Mesh mesh)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var normalizedIndex = paramTex.GetNormalizedIndex(this);

            {
                var count = mesh.vertexCount;
                var uvs = mesh.uv;
                for (var i = 0; i < count; i++)
                {
                    var uv = uvs[i];
                    uvs[i] = new Vector2(
                        Packer.ToFloat((uv.x + 0.5f) / 2f, (uv.y + 0.5f) / 2f),
                        normalizedIndex
                    );
                }
                mesh.SetUVs(0, uvs);
            }
        }

        protected override void SetEffectParamsDirty()
        {
            paramTex.SetData(this, 1, m_ColorFactor); // param.y : color factor
        }
    }
}
