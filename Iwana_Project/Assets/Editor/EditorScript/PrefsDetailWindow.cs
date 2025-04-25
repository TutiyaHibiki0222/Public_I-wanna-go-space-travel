#if UNITY_EDITOR
/* 
    - 2025 2/04 土屋 響 制作 
    PlayerPrefsEditor から 開かれた PlayerPrefsを編集する EditorWindow.
*/


using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using System.Text;

public class PrefsDetailWindow : EditorWindow
{
    // 親ウィンドウの参照を保持
    PlayerPrefsEditor playerPrefsEditor;

    string key;
    string initValue;
    string resetValue;
    string value;
    string newKey;
    ValueType selectedValueType;
    ValueType initselectedValueType;

    // スクロールビューのスクロール位置
    private Vector2 scrollPosition;
    private Vector2 previewScrollPosition;

    bool isKeyChanged;
    bool isTypeChanged;
    bool isValueInitialized = false;
    bool drawJson;

    // PrefsDetailWindowを表示するメソッド.
    public static void ShowWindow(string key, string value, ValueType keyType, PlayerPrefsEditor editor)
    {
        PrefsDetailWindow window = GetWindow<PrefsDetailWindow>("Edit Prefs");
        window.key = key;
        window.value = value;
        window.initValue = value;
        window.selectedValueType = keyType;
        window.initselectedValueType = keyType;
        window.newKey = key;
        window.isKeyChanged = false;  
        window.isTypeChanged = false; 
        window.playerPrefsEditor = editor;
        window.Show();
    }

    // 編集画面のUIを作成
    private void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField($"Edit Prefs [{key}]", EditorStyles.boldLabel);
        // 保存ボタン
        if (GUILayout.Button("Save Changes", GUILayout.Width(300)))
        {
            SaveChanges();
        }
        GUILayout.EndHorizontal();
        GUIStyle lineStyle = new GUIStyle();
        lineStyle.normal.background = EditorGUIUtility.whiteTexture;
        lineStyle.margin = new RectOffset(0, 0, 4, 4);
        lineStyle.fixedHeight = 2;

        Color lineColor = new Color(126f / 255f, 126f / 255f, 126f / 255f, 1.0f);
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), lineColor);
        // Keyの変更の有無を選択するチェックボックス
        isKeyChanged = EditorGUILayout.Toggle("Change Key", isKeyChanged);

        // Keyの編集フィールド（キーの変更を可能にする）
        if (isKeyChanged)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("New Key: ", GUILayout.Width(60));
            newKey = GUILayout.TextField(newKey, GUILayout.Width(300));  // 新しいキーの入力フィールド
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            newKey = key;
            GUILayout.Label("Key: ", GUILayout.Width(60));
            GUILayout.Label(newKey, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();
        }

        // 型の変更の有無を選択するチェックボックス
        isTypeChanged = EditorGUILayout.Toggle("Change Type", isTypeChanged);

        // 型選択のフィールド（型の変更を可能にする）
        if (isTypeChanged)
        {
            selectedValueType = (ValueType)EditorGUILayout.EnumPopup("New Value Type", selectedValueType);
            if (!isValueInitialized)
            {
                isValueInitialized = true;
                resetValue = value;
            }
        }
        else
        {
            if (isValueInitialized)
            {
                isValueInitialized = false;
                value = resetValue;
            }
            EditorGUILayout.BeginHorizontal();
            selectedValueType = initselectedValueType;
            GUILayout.Label("Value Type: ", GUILayout.Width(150));
            GUILayout.Label(selectedValueType.ToString(), GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        }

        // selectedValueType が STRING なおかつ Jsonに変換できるか?
        if (selectedValueType == ValueType.STRING && IsJsonValid(value))
             drawJson = EditorGUILayout.Toggle("Draw Json", drawJson);
        else drawJson = false;

        // 
        if (drawJson)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
            // JSON形式のデータを表示.
            try
            {
                // valueがJSON形式だと仮定して、デシリアライズし整形
                var parsedJson = JsonConvert.DeserializeObject(value);

                // 整形されたJSON（インデントあり）を表示
                var formattedJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                formattedJson = EditorGUILayout.TextArea(formattedJson);  // 複数行の入力エリア

                // ユーザーが編集した後の整形されたJSONを保存
                value = formattedJson;

                // 保存前に一行にまとめる
                var compactJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(value), Formatting.None);
                value = compactJson;
            }
            catch (JsonException ex)
            {
                // JSONパースエラーが発生した場合
                EditorGUILayout.LabelField("Invalid JSON", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(ex.Message);
                value = EditorGUILayout.TextArea(value);
            }
        }
        else
        {
            // スクロール可能な領域.
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
            // ここで入力エリアの高さを制限する
            float maxLines = 10f;    // 最大行数
            float lineHeight = 16f;  // 各行の高さ
            float textAreaHeight = Mathf.Min(value.Split('\n').Length, maxLines) * lineHeight;

            // 複数行の入力エリア
            switch (selectedValueType)
            {
                case ValueType.STRING:
                    value = EditorGUILayout.TextArea(value, GUILayout.Height(textAreaHeight));
                    break;
                case ValueType.INT:
                    value = EditorGUILayout.TextArea(value, GUILayout.Height(textAreaHeight)); 
                    value = Regex.Replace(value, "[^0-9]", "");
                    break;
                case ValueType.FLOAT:
                    value = EditorGUILayout.TextArea(value, GUILayout.Height(textAreaHeight));
                    value = Regex.Replace(value, @"[^0-9.]", "");
                    int dotIndex = value.IndexOf('.');
                    if (dotIndex != -1)
                    {
                        value = value.Substring(0, dotIndex + 1) + value.Substring(dotIndex + 1).Replace(".", "");
                    }
                    break;

            }
        }
        // 1つ目のスクロールビューを終了.
        EditorGUILayout.EndScrollView();    
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), lineColor);
        EditorGUILayout.Space();

        GUILayout.Label("Changes Preview:");
        // 変更内容を表示するためのスクロールビュー
        previewScrollPosition = EditorGUILayout.BeginScrollView(previewScrollPosition, GUILayout.Height(150));

        // Keyの変更があれば表示（青）
        if (isKeyChanged)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Key: {key} -> <color=#3399FF>{newKey}</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), new Color(0.2f, 0.6f, 1.0f, 1.0f)); // 青
            GUILayout.EndVertical();
        }

        // 型の変更があれば表示（オレンジ）
        if (isTypeChanged)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Type: {initselectedValueType} -> <color=#FF9933>{selectedValueType}</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), new Color(1.0f, 0.6f, 0.2f, 1.0f)); // オレンジ
            GUILayout.EndVertical();
        }

        // 値の変更があれば表示（緑）
        if (value != initValue)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Value: {initValue} \n ↓ \n<color=#33CC33>Value: {value}</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), new Color(0.2f, 1.0f, 0.2f, 1.0f)); // 緑
            GUILayout.EndVertical();
        }

        // すべての変更がない場合にデフォルト状態を表示する場合.
        if (!isKeyChanged && !isTypeChanged && value == initValue)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("No Changes", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }
    /// 差分をハイライトするメソッド
    public string HighlightChanges(string original, string modified)
    {
        int minLength = Mathf.Min(original.Length, modified.Length);
        StringBuilder result = new StringBuilder();

        // 文字ごとの比較
        for (int i = 0; i < minLength; i++)
        {
            if (original[i] == modified[i])
            {
                // 変更なし
                result.Append(modified[i]);
            }
            else
            {
                // 変更された部分はオレンジ色
                result.Append($"<color=#FFA500><b>{modified[i]}</b></color>");
            }
        }

        // 削除された部分（元の文字列にあって新しい文字列にない部分）
        if (original.Length > modified.Length)
        {
            result.Append($"<color=#FF6347><b>{original.Substring(minLength)}</b></color>"); // 赤色で表示
        }

        // 追加された部分（新しい文字列にあって元の文字列にない部分）
        if (modified.Length > original.Length)
        {
            result.Append($"<color=#32CD32><b>{modified.Substring(minLength)}</b></color>"); // 青色で表示
        }

        return result.ToString();
    }
    /// <summary>
    /// PlayerPrefs 保存し 閉じる.
    /// </summary>
    private new void SaveChanges()
    {
        bool validType = true;

        // 新しいキー名をセット
        if (isKeyChanged && newKey != key && PlayerPrefs.HasKey(newKey))
        {
            bool overwrite = EditorUtility.DisplayDialog("Key Already Exists", $"The key \"{newKey}\" already exists. Do you want to overwrite it?", "Yes", "No");
            if (!overwrite)
            {
                return;
            }
        }

        try
        {
            // 型に基づいて保存処理を行う
            if (selectedValueType == ValueType.INT)
            {
                int intValue = int.Parse(value);

                // 古いキーを削除して新しいキーで保存
                if (key != newKey) PlayerPrefs.DeleteKey(key);
                PlayerPrefs.SetInt(newKey, intValue);
            }
            else if (selectedValueType == ValueType.FLOAT)
            {
                float floatValue = float.Parse(value);

                // 古いキーを削除して新しいキーで保存
                if (key != newKey) PlayerPrefs.DeleteKey(key);
                PlayerPrefs.SetFloat(newKey, floatValue);
            }
            else if (selectedValueType == ValueType.STRING)
            {
                // 古いキーを削除して新しいキーで保存
                if (key != newKey) PlayerPrefs.DeleteKey(key);
                PlayerPrefs.SetString(newKey, value);
            }
            else
            {
                validType = false;
            }
        }
        catch
        {
            validType = false;
        }

        if (validType)
        {
            PlayerPrefs.Save();
            // 編集後のキーと値が保存された
            Debug.Log("Changes Saved");
            playerPrefsEditor.LoadPlayerPrefs();
            Close();
        }
        else
        {
            EditorUtility.DisplayDialog("無効な入力", "選択されたキータイプと入力された値が一致しません。", "OK");
        }
    }
    /// <summary>
    /// Jsonに変換可能か.
    /// </summary>
    /// <param name="value"> 要素 </param>
    /// <returns> 可能なら true それ以外 false </returns>
    bool IsJsonValid(string value)
    {
        try
        {
            // JSON としてパースを試みる
            JsonConvert.DeserializeObject(value);
            return true;  // エラーが発生しなければ、有効なJSON
        }
        catch (JsonException)
        {
            return false;  // JSON 形式でない場合はエラーが発生
        }
    }
}

#endif
