using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectSearchWindow : EditorWindow
{
    private string searchQuery = ""; // �����N�G��
    private MonoScript selectedScript; // �h���b�O���h���b�v�őI�����ꂽ�X�N���v�g
    private Vector2 scrollPosition;
    private GameObject[] searchResults;

    [MenuItem("Tools/Object Search")]
    public static void ShowWindow()
    {
        GetWindow<ObjectSearchWindow>("Object Search");
    }

    private void OnGUI()
    {
        GUILayout.Label("Object Search", EditorStyles.boldLabel);

        // �����o�[
        GUILayout.BeginHorizontal();
        searchQuery = EditorGUILayout.TextField("Name", searchQuery);
        GUILayout.EndHorizontal();
        // �h���b�O���h���b�v�ŃX�N���v�g�I��
        GUILayout.Space(10);
        GUILayout.Label("Drag & Drop a Script Here", EditorStyles.boldLabel);
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop Script Here", EditorStyles.helpBox);

        HandleScriptDrop(dropArea);

        // �X�N���v�g���̕\��
        if (selectedScript != null)
        {
            GUILayout.Label($"Selected Script: {selectedScript.name}", EditorStyles.label);
        }

        // �����{�^��
        if (GUILayout.Button("Search"))
        {
            SearchObjects();
        }

        // �������ʂ̕\��
        if (searchResults != null && searchResults.Length > 0)
        {
            GUILayout.Label($"Found {searchResults.Length} objects:", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var obj in searchResults)
            {
                EditorGUILayout.ObjectField(obj, typeof(GameObject), false);
            }

            GUILayout.EndScrollView();
        }
    }

    private void HandleScriptDrop(Rect dropArea)
    {
        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        MonoScript script = DragAndDrop.objectReferences[0] as MonoScript;
                        if (script != null)
                        {
                            selectedScript = script;
                        }
                    }

                    evt.Use();
                }
                break;
        }
    }

    private void SearchObjects()
    {
        if (string.IsNullOrEmpty(searchQuery) && selectedScript == null)
        {
            searchResults = null;
            return;
        }

        // �V�[�����̂��ׂĂ�GameObject������
        var allObjects = GameObject.FindObjectsOfType<GameObject>();

        searchResults = System.Array.FindAll(allObjects, obj =>
        {
            // ���O�ōi�荞��
            bool matchesName = string.IsNullOrEmpty(searchQuery) || obj.name.ToLower().Contains(searchQuery.ToLower());

            // �X�N���v�g�ōi�荞��
            bool matchesScript = selectedScript == null || obj.GetComponent(selectedScript.GetClass()) != null;

            return matchesName && matchesScript;
        });
    }

    private void OnFocus()
    {
        // �V�[���r���[�̍X�V������
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnLostFocus()
    {
        // �V�[���r���[�C�x���g�������
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (searchResults == null || searchResults.Length == 0)
            return;

        Handles.color = Color.cyan; // �~�̐F

        foreach (var obj in searchResults)
        {
            if (obj == null) continue;

            float radius = 0.25f * Mathf.Max(obj.transform.localScale.x, obj.transform.localScale.y); // �~�̔��a
            Handles.DrawWireDisc(obj.transform.position, Vector3.forward, radius);

            // ���x����`��
            Handles.Label(obj.transform.position, obj.name, EditorStyles.boldLabel);
        }

        // �V�[���r���[���ĕ`��
        SceneView.RepaintAll();
    }
}
