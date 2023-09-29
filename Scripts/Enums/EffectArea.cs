using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    /// <summary>
    /// Area for effect.
    /// </summary>
    public enum EffectArea
    {
        RectTransform,
        Fit,
        Character,
    }

    public static class EffectAreaExtensions
    {
        static readonly Rect rectForCharacter = new Rect(0, 0, 1, 1);

        /// <summary>
        /// Gets effect for area.
        /// </summary>
        public static Rect GetEffectArea(this EffectArea area, Mesh mesh, Rect rectangle, float aspectRatio = -1)
        {
            var vertices = mesh.vertices;

            Rect rect = default(Rect);
            switch (area)
            {
                case EffectArea.RectTransform:
                    rect = rectangle;
                    break;
                case EffectArea.Character:
                    rect = rectForCharacter;
                    break;
                case EffectArea.Fit:
                    // Fit to contents.
                    float xMin = float.MaxValue;
                    float yMin = float.MaxValue;
                    float xMax = float.MinValue;
                    float yMax = float.MinValue;
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        var vertex = vertices[i];
                        float x = vertex.x;
                        float y = vertex.y;
                        xMin = Mathf.Min(xMin, x);
                        yMin = Mathf.Min(yMin, y);
                        xMax = Mathf.Max(xMax, x);
                        yMax = Mathf.Max(yMax, y);
                    }

                    rect.Set(xMin, yMin, xMax - xMin, yMax - yMin);
                    break;
                default:
                    rect = rectangle;
                    break;
            }


            if (0 < aspectRatio)
            {
                if (rect.width < rect.height)
                {
                    rect.width = rect.height * aspectRatio;
                }
                else
                {
                    rect.height = rect.width / aspectRatio;
                }
            }

            return rect;
        }

        /// <summary>
        /// Gets effect for area.
        /// </summary>
        public static Rect GetEffectArea(this EffectArea area, MeshBuilder mb, Rect rectangle, float aspectRatio = -1)
        {
            Rect rect = default(Rect);
            switch (area)
            {
                case EffectArea.RectTransform:
                    rect = rectangle;
                    break;
                case EffectArea.Character:
                    rect = rectForCharacter;
                    break;
                case EffectArea.Fit:
                    rect = mb.Poses.CalculateBoundingRect();
                    break;
                default:
                    rect = rectangle;
                    break;
            }


            if (0 < aspectRatio)
            {
                if (rect.width < rect.height)
                {
                    rect.width = rect.height * aspectRatio;
                }
                else
                {
                    rect.height = rect.width / aspectRatio;
                }
            }

            return rect;
        }
    }
}
