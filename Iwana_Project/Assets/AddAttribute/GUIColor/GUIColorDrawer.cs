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
            // �������擾
            GUIColorAttribute guiColor = (GUIColorAttribute)attribute;

            // ���݂� GUI �F��ۑ�
            Color previousColor = GUI.color;

            // �����Ŏw�肳�ꂽ�F�ɕύX
            GUI.color = guiColor.color;

            // �v���p�e�B��`��
            EditorGUI.PropertyField(position, property, label, true);

            // GUI �F�����ɖ߂�
            GUI.color = previousColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // �ʏ�̃v���p�e�B������Ԃ�
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
#endif
}
