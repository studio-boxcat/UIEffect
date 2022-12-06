using UnityEditor;

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIEffect editor.
    /// </summary>
    [CustomEditor(typeof(UIEffect))]
    [CanEditMultipleObjects]
    public class UIEffectEditor : Editor
    {
        SerializedProperty _spColorMode;
        SerializedProperty _spColorFactor;

        protected void OnEnable()
        {
            _spColorMode = serializedObject.FindProperty("m_ColorMode");
            _spColorFactor = serializedObject.FindProperty("m_ColorFactor");
        }

        public override void OnInspectorGUI()
        {
            //================
            // Color setting.
            //================
            using (new MaterialDirtyScope(targets))
                EditorGUILayout.PropertyField(_spColorMode);

            // When color is enable, show parameters.
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_spColorFactor);
                EditorGUI.indentLevel--;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
