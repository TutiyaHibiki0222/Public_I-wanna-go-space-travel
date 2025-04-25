#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Attribute.Add
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonEditor : Editor
    {
        // ボタン描画処理
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var targetObject = target;
            // 対象オブジェクトの全メソッドを取得
            var methods = targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // ButtonAttribute が付与されたメソッドを探す
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute != null)
                {
                    // ボタンのラベルを設定 (buttonNameがnullの場合はメソッド名を使用)
                    string buttonLabel = string.IsNullOrEmpty(buttonAttribute.buttonName) ? method.Name : buttonAttribute.buttonName;

                    // ボタンを描画
                    if (GUILayout.Button(buttonLabel))
                    {
                        // メソッドを呼び出す
                        method.Invoke(targetObject, null);
                    }
                }
            }
        }
    }
}
#endif
