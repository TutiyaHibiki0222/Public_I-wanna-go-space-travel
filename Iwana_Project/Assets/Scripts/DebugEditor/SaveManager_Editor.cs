#if UNITY_EDITOR
using Common;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager), true)]
public class SaveManager_Editor : Editor
{
    SaveManager thisScript;
    [SerializeField, Range(0, Config.saveMax)] private int index = 0;
    void Awake()
    {
        thisScript = target as SaveManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Editor���SaveManager�֐��𔭓������p�̃{�^���ł��B");

        if (GUILayout.Button("Save"))
        {
            thisScript.ScreenShot_OR_Save();
        }

        index = EditorGUILayout.IntField("�I�[�v�� �Z�[�u���X�g", index);

        if (GUILayout.Button("Open_Save"))
        {
            thisScript.OpenSave(index);
        }

        if (GUILayout.Button("Delete_Save"))
        {
            thisScript.DeleteSave(index);
        }
    }
}
#endif