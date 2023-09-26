using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// Abstract effect base for UI.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseMaterialEffect : BaseMeshEffect, IParameterInstance, IMaterialModifier
    {
        ulong _lastMaterialHash;

        /// <summary>
        /// Gets or sets the parameter index.
        /// </summary>
        int IParameterInstance.index { get; set; }

        /// <summary>
        /// Gets the parameter texture.
        /// </summary>
        public virtual ParameterTexture paramTex
        {
            get { return null; }
        }

        /// <summary>
        /// Mark the vertices as dirty.
        /// </summary>
        public void SetMaterialDirty()
        {
            if (graphic)
                graphic.SetMaterialDirty();
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled) return baseMaterial;

            var oldMaterialHash = _lastMaterialHash;

            var newMaterialHash = _lastMaterialHash = GetMaterialHash(baseMaterial);
            if (MaterialCache.TryRent(newMaterialHash, out var material) == false)
            {
                material = CreateMaterial(baseMaterial);
                MaterialCache.RegisterNewlyRented(newMaterialHash, material);
            }

            if (oldMaterialHash != default)
                MaterialCache.Return(oldMaterialHash);

            return material;
        }

        // protected bool isTMProMobile (Material material)
        // {
        // 	return material && material.shader && material.shader.name.StartsWith ("TextMeshPro/Mobile/", StringComparison.Ordinal);
        // }

        protected abstract ulong GetMaterialHash(Material baseMaterial);

        protected abstract Material CreateMaterial(Material baseMaterial);

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

            // foreach (var mr in GetComponentsInChildren<UIEffectMaterialResolver> ())
            // {
            // 	mr.GetComponent<Graphic> ().SetMaterialDirty ();
            // 	mr.GetComponent<Graphic> ().SetVerticesDirty ();
            // }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            SetMaterialDirty();

            paramTex?.Unregister(this);

            if (_lastMaterialHash != default)
            {
                MaterialCache.Return(_lastMaterialHash);
                _lastMaterialHash = default;
            }
        }

        // protected override void OnDidApplyAnimationProperties()
        // {
        //     SetEffectParamsDirty();
        // }

        // protected override void OnTextChanged (UnityEngine.Object obj)
        // {
        // 	base.OnTextChanged (obj);
        //
        //
        // 	foreach (var sm in GetComponentsInChildren<TMPro.TMP_SubMeshUI> ())
        // 	{
        // 		if(!sm.GetComponent<UIEffectMaterialResolver>())
        // 		{
        // 			var mr = sm.gameObject.AddComponent<UIEffectMaterialResolver> ();
        //
        // 			targetGraphic.SetAllDirty ();
        // 			//targetGraphic.SetVerticesDirty ();
        //
        // 			//mr.GetComponent<Graphic> ().SetMaterialDirty ();
        // 			//mr.GetComponent<Graphic> ().SetVerticesDirty ();
        //
        //
        // 		}
        // 	}
        // }
    }
}