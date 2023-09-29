using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// Base class for effects that modify the generated Mesh.
    /// It works well not only for standard Graphic components (Image, RawImage, Text, etc.) but also for TextMeshPro and TextMeshProUGUI.
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public abstract class BaseMeshEffect : UIBehaviour, IMeshModifier
    {
        [NonSerialized] RectTransform _rectTransform;
        [NonSerialized] Graphic _graphic;

        /// <summary>
        /// The Graphic attached to this GameObject.
        /// </summary>
        public Graphic graphic => _graphic ??= GetComponent<Graphic>();

        /// <summary>
        /// The RectTransform attached to this GameObject.
        /// </summary>
        protected RectTransform rectTransform => _rectTransform ??= (RectTransform) transform;

        /// <summary>
        /// Call used to modify mesh.
        /// </summary>
        /// <param name="mb">VertexHelper.</param>
        public abstract void ModifyMesh(MeshBuilder mb);

        /// <summary>
        /// Mark the vertices as dirty.
        /// </summary>
        protected virtual void SetVerticesDirty()
        {
            if (graphic)
                graphic.SetVerticesDirty();
        }


        //################################
        // Protected Members.
        //################################
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            SetVerticesDirty();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            SetVerticesDirty();
        }

        /// <summary>
        /// Mark the effect parameters as dirty.
        /// </summary>
        protected virtual void SetEffectParamsDirty()
        {
            if (!isActiveAndEnabled) return;
            SetVerticesDirty();
        }

        /// <summary>
        /// Callback for when properties have been changed by animation.
        /// </summary>
        protected virtual void OnDidApplyAnimationProperties()
        {
            if (!isActiveAndEnabled) return;
            SetEffectParamsDirty();
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            if (!isActiveAndEnabled) return;
            SetVerticesDirty();
        }

        /// <summary>
        /// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        /// </summary>
        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled) return;
            SetEffectParamsDirty();
        }
#endif
    }
}