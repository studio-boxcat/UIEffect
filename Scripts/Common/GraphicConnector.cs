using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Coffee.UIEffects
{
    public class GraphicConnector
    {
        private static readonly List<GraphicConnector> s_Connectors = new List<GraphicConnector>();

        private static readonly Dictionary<Type, GraphicConnector> s_ConnectorMap =
            new Dictionary<Type, GraphicConnector>();

        private static readonly GraphicConnector s_EmptyConnector = new GraphicConnector();

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            AddConnector(new GraphicConnector());
        }

        protected static void AddConnector(GraphicConnector connector)
        {
            s_Connectors.Add(connector);
            s_Connectors.Sort((x, y) => y.priority - x.priority);
        }

        public static GraphicConnector FindConnector(Graphic graphic)
        {
            if (!graphic) return s_EmptyConnector;

            var type = graphic.GetType();
            GraphicConnector connector = null;
            if (s_ConnectorMap.TryGetValue(type, out connector)) return connector;

            foreach (var c in s_Connectors)
            {
                if (!c.IsValid(graphic)) continue;

                s_ConnectorMap.Add(type, c);
                return c;
            }

            return s_EmptyConnector;
        }

        /// <summary>
        /// Connector priority.
        /// </summary>
        protected virtual int priority
        {
            get { return -1; }
        }

        /// <summary>
        /// The connector is valid for the component.
        /// </summary>
        protected virtual bool IsValid(Graphic graphic)
        {
            return true;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        public virtual void OnEnable(Graphic graphic)
        {
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        public virtual void OnDisable(Graphic graphic)
        {
        }

        /// <summary>
        /// Mark the vertices as dirty.
        /// </summary>
        public virtual void SetVerticesDirty(Graphic graphic)
        {
            if (graphic)
                graphic.SetVerticesDirty();
        }

        /// <summary>
        /// Mark the material as dirty.
        /// </summary>
        public virtual void SetMaterialDirty(Graphic graphic)
        {
            if (graphic)
                graphic.SetMaterialDirty();
        }

        /// <summary>
        /// Normalize vertex position by local matrix.
        /// </summary>
        public virtual void GetNormalizedFactor(EffectArea area, int index, Matrix2x3 matrix, Vector2 position,
            out Vector2 normalizedPos)
        {
            normalizedPos = matrix * position;
        }
    }
}
