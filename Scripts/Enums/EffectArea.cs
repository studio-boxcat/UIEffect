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
        public static Rect GetEffectArea(this EffectArea area, VertexHelper vh, Rect rectangle, float aspectRatio = -1)
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
                    // Fit to contents.
                    UIVertex vertex = default(UIVertex);
                    float xMin = float.MaxValue;
                    float yMin = float.MaxValue;
                    float xMax = float.MinValue;
                    float yMax = float.MinValue;
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vertex, i);
                        float x = vertex.position.x;
                        float y = vertex.position.y;
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
    }
}
