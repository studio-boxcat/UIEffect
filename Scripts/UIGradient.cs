using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// UIGradient.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UIEffects/UIGradient", 101)]
    public class UIGradient : BaseMeshEffect
    {
        /// <summary>
        /// Gradient direction.
        /// </summary>
        public enum Direction
        {
            Horizontal,
            Vertical,
        }

        [Tooltip("Gradient Direction.")] [SerializeField]
        Direction m_Direction;

        [Tooltip("Color1: Top or Left.")] [SerializeField]
        Color m_Color1 = Color.white;

        [Tooltip("Color2: Bottom or Right.")] [SerializeField]
        Color m_Color2 = Color.white;

        [Tooltip("Gradient offset for Horizontal, Vertical or Angle.")] [SerializeField] [Range(-1, 1)]
        float m_Offset1;

        [Tooltip("Color space to correct color.")] [SerializeField]
        [HideIf("@m_ColorSpace == ColorSpace.Uninitialized")] // Will be removed.
        ColorSpace m_ColorSpace = ColorSpace.Uninitialized;

        /// <summary>
        /// Gradient Direction.
        /// </summary>
        public Direction direction
        {
            get => m_Direction;
            set
            {
                if (m_Direction == value) return;
                m_Direction = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Color1: Top or Left.
        /// </summary>
        public Color color1
        {
            get => m_Color1;
            set
            {
                if (m_Color1 == value) return;
                m_Color1 = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Color2: Bottom or Right.
        /// </summary>
        public Color color2
        {
            get => m_Color2;
            set
            {
                if (m_Color2 == value) return;
                m_Color2 = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Gradient offset for Horizontal, Vertical or Angle.
        /// </summary>
        public float offset
        {
            get { return m_Offset1; }
            set
            {
                if (Mathf.Approximately(m_Offset1, value)) return;
                m_Offset1 = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Call used to modify mesh.
        /// </summary>
        public override void ModifyMesh(MeshBuilder mb)
        {
            // Gradient space.
            var rect = graphic.rectTransform.rect;

            // Calculate min max range.
            var (min, max) = m_Direction == Direction.Horizontal
                ? (rect.xMin, rect.xMax)
                : (rect.yMin, rect.yMax);

            var vertCount = mb.Poses.Count;
            var poses = mb.Poses;
            var colors = mb.Colors.Edit();

            for (var i = 0; i < vertCount; i++)
            {
                // Normalize vertex position.
                var pos = m_Direction == Direction.Horizontal ? poses[i].x : poses[i].y;
                var normalizedPos = Mathf.InverseLerp(min, max, pos) + m_Offset1;

                // Interpolate vertex color.
                var color = Color.Lerp(m_Color2, m_Color1, normalizedPos);

                // Correct color.
                colors[i] *= m_ColorSpace switch
                {
                    ColorSpace.Gamma => color.gamma,
                    ColorSpace.Linear => color.linear,
                    _ => color
                };
            }
        }
    }
}