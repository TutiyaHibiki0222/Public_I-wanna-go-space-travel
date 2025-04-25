using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Attribute.Add
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            // SerializedProperty のパスを取得し、条件のフィールドを探索
            string conditionPath = property.propertyPath.Replace(property.name, showIf.ConditionName);
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionPath);

            bool shouldShow = false;

            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
            {
                // 条件フィールドの値を取得
                shouldShow = conditionProperty.boolValue;
            }
            else
            {
                Debug.LogWarning($"ShowIf: Condition '{showIf.ConditionName}' not found or invalid. It must be a bool field or method.");
            }

            if (shouldShow)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else if (showIf.HideMode == HideMode.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            string conditionPath = property.propertyPath.Replace(property.name, showIf.ConditionName);
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionPath);

            bool shouldShow = false;

            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
            {
                shouldShow = conditionProperty.boolValue;
            }

            return shouldShow || showIf.HideMode != HideMode.Hide ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
        }
    }
#endif
}
