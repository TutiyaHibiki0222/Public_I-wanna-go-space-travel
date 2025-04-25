using UnityEngine;
using UnityEditor;
public class MapEditorWindow : EditorWindow
{
    [MenuItem("Tools/Map Editor")]
    public static void ShowWindow()
    {
        // title–¼.
        GetWindow<MapEditorWindow>("Map Editor");
    }

    private void OnGUI()
    {
        // UI•\Ž¦.
        GUILayout.Label("Map Editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Save Map"))
        {
            SaveMap();
        }

        if (GUILayout.Button("Load Map"))
        {
            LoadMap();
        }
    }

    private void SaveMap()
    {
        Debug.Log("Save Map to JSON");

    }

    private void LoadMap()
    {
        Debug.Log("Load Map from JSON");

    }
}
