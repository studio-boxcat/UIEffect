using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// Abstract effect base for UI.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseMaterialEffect : BaseMeshEffect, IParameterInstance, IMaterialModifier
    {
        /// <summary>
        /// Gets or sets the parameter index.
        /// </summary>
        int IParameterInstance.index { get; set; }

        /// <summary>
        /// Gets the parameter texture.
        /// </summary>
        public abstract ParameterTexture paramTex { get; }

        /// <summary>
        /// Mark the vertices as dirty.
        /// </summary>
        public void SetMaterialDirty() => graphic.SetMaterialDirty();

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (enabled is false) return baseMaterial;
            var effectMat = GetEffectMaterial(baseMaterial);
            Assert.IsNotNull(effectMat.GetTexture("_ParamTex"), "Material must have a texture property '_ParamTex'.");
            return effectMat;
        }

        protected abstract Material GetEffectMaterial(Material baseMaterial);

#if UNITY_EDITOR
        protected override void Reset()
        {
            if (!isActiveAndEnabled) return;
            SetMaterialDirty();
            SetVerticesDirty();
            SetEffectParamsDirty();
        }

        protected override void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            SetVerticesDirty();
            SetEffectParamsDirty();
        }
#endif

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            paramTex?.Register(this);

            SetMaterialDirty();
            SetEffectParamsDirty();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            SetMaterialDirty();

            paramTex?.Unregister(this);
        }
    }
}