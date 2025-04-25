#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Attribute.Add
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonEditor : Editor
    {
        // �{�^���`�揈��
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var targetObject = target;
            // �ΏۃI�u�W�F�N�g�̑S���\�b�h���擾
            var methods = targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // ButtonAttribute ���t�^���ꂽ���\�b�h��T��
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute != null)
                {
                    // �{�^���̃��x����ݒ� (buttonName��null�̏ꍇ�̓��\�b�h�����g�p)
                    string buttonLabel = string.IsNullOrEmpty(buttonAttribute.buttonName) ? method.Name : buttonAttribute.buttonName;

                    // �{�^����`��
                    if (GUILayout.Button(buttonLabel))
                    {
                        // ���\�b�h���Ăяo��
                        method.Invoke(targetObject, null);
                    }
                }
            }
        }
    }
}
#endif
