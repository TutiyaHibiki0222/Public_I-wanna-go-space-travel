using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectSearchWindow : EditorWindow
{
    private string searchQuery = ""; // 検索クエリ
    private MonoScript selectedScript; // ドラッグ＆ドロップで選択されたスクリプト
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

        // 検索バー
        GUILayout.BeginHorizontal();
        searchQuery = EditorGUILayout.TextField("Name", searchQuery);
        GUILayout.EndHorizontal();
        // ドラッグ＆ドロップでスクリプト選択
        GUILayout.Space(10);
        GUILayout.Label("Drag & Drop a Script Here", EditorStyles.boldLabel);
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop Script Here", EditorStyles.helpBox);

        HandleScriptDrop(dropArea);

        // スクリプト名の表示
        if (selectedScript != null)
        {
            GUILayout.Label($"Selected Script: {selectedScript.name}", EditorStyles.label);
        }

        // 検索ボタン
        if (GUILayout.Button("Search"))
        {
            SearchObjects();
        }

        // 検索結果の表示
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

        // シーン内のすべてのGameObjectを検索
        var allObjects = GameObject.FindObjectsOfType<GameObject>();

        searchResults = System.Array.FindAll(allObjects, obj =>
        {
            // 名前で絞り込み
            bool matchesName = string.IsNullOrEmpty(searchQuery) || obj.name.ToLower().Contains(searchQuery.ToLower());

            // スクリプトで絞り込み
            bool matchesScript = selectedScript == null || obj.GetComponent(selectedScript.GetClass()) != null;

            return matchesName && matchesScript;
        });
    }

    private void OnFocus()
    {
        // シーンビューの更新を強制
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnLostFocus()
    {
        // シーンビューイベントから解除
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (searchResults == null || searchResults.Length == 0)
            return;

        Handles.color = Color.cyan; // 円の色

        foreach (var obj in searchResults)
        {
            if (obj == null) continue;

            float radius = 0.25f * Mathf.Max(obj.transform.localScale.x, obj.transform.localScale.y); // 円の半径
            Handles.DrawWireDisc(obj.transform.position, Vector3.forward, radius);

            // ラベルを描画
            Handles.Label(obj.transform.position, obj.name, EditorStyles.boldLabel);
        }

        // シーンビューを再描画
        SceneView.RepaintAll();
    }
}
