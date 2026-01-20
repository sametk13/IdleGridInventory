#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace OmniGameTemplate.Core.Data.Variables.Editor
{
    /// <summary>
    /// Shared UI: draws (Use Constant) toggle, constant field, and variable field on one line (folds if needed).
    /// </summary>
    public abstract class VariableReferenceDrawerBase : PropertyDrawer
    {
        protected const float ToggleWidth = 110f; // "Use Constant"
        protected const float Spacing = 4f;

        protected abstract string ConstantFieldName { get; }
        protected abstract string VariableFieldName { get; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Let Unity calculate properly (handles wrap/fold)
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty useConstantProp = property.FindPropertyRelative("useConstant");
            SerializedProperty constantProp    = property.FindPropertyRelative(ConstantFieldName);
            SerializedProperty variableProp    = property.FindPropertyRelative(VariableFieldName);

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var toggleRect   = new Rect(position.x, position.y, ToggleWidth, EditorGUIUtility.singleLineHeight);
            var rightRect    = new Rect(position.x + ToggleWidth + Spacing, position.y,
                                        position.width - ToggleWidth - Spacing, EditorGUIUtility.singleLineHeight);

            useConstantProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Use Constant", useConstantProp.boolValue);

            if (useConstantProp.boolValue)
            {
                EditorGUI.PropertyField(rightRect, constantProp, GUIContent.none, true);
            }
            else
            {
                EditorGUI.PropertyField(rightRect, variableProp, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
