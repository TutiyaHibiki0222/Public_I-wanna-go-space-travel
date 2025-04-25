using UnityEditor;
using UnityEngine;

namespace Attribute.Add
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GUIColorAttribute))]
    public class GUIColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 属性を取得
            GUIColorAttribute guiColor = (GUIColorAttribute)attribute;

            // 現在の GUI 色を保存
            Color previousColor = GUI.color;

            // 属性で指定された色に変更
            GUI.color = guiColor.color;

            // プロパティを描画
            EditorGUI.PropertyField(position, property, label, true);

            // GUI 色を元に戻す
            GUI.color = previousColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 通常のプロパティ高さを返す
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
#endif
}
