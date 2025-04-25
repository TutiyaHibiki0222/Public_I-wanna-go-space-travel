#if UNITY_EDITOR
/* 
    - 2025 2/04 土屋 響 制作 
    PlayerPrefs を EditorWindowを使用して管理

    class PlayerPrefsEditor;
*/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public enum ValueType
{
    INT,
    FLOAT,
    STRING
}

public class PlayerPrefsEditor : EditorWindow
{
    private List<string> prefsKeys = new List<string>();
    private Dictionary<string, object> prefsData = new Dictionary<string, object>();
    private Vector2 scrollPos;
    private bool showSettings = true;

    // 現在の選択されたキータイプ
    private ValueType selectedValueType = ValueType.STRING;
    private string newKey = "";
    private string newValue = "";
    // メッセージの色を個別に定義
    Color messageColorKey = Color.black;
    Color messageColorValue = Color.black;

    // メッセージの内容を個別に定義
    string messageKey = "";
    string messageValue = "";
    // 除外するキーのリスト（HashSetで効率的に管理）
    private static HashSet<string> excludedKeys = new HashSet<string>
    {
        "unity.player_sessionid",
        "unity.player_session_count",
        "unity.cloud_userid",
        "UnityGraphicsQuality"
    };

    [MenuItem("Tools/PlayerPrefs Manager")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsEditor>("PlayerPrefs Manager");
    }

    private void OnEnable()
    {
        LoadPlayerPrefs();
    }

    public void LoadPlayerPrefs()
    {
        PlayerPrefsTools.GetAllPlayerPrefKeys(ref prefsKeys);

        prefsData.Clear();
        // 除外キーを除いたキーのみを処理
        foreach (var key in prefsKeys.Where(key => !excludedKeys.Contains(key)))
        {
            prefsData[key] = LoadPrefValue(key);
        }
    }

    private object LoadPrefValue(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return null;
        }

        string stringValue = PlayerPrefs.GetString(key, null);
        if (!string.IsNullOrEmpty(stringValue))
        {
            return stringValue;
        }

        if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue)
        {
            return PlayerPrefs.GetInt(key);
        }

        if (PlayerPrefs.GetFloat(key, float.MinValue) != float.MinValue)
        {
            return PlayerPrefs.GetFloat(key);
        }

        return null;
    }


    private void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Manager", EditorStyles.boldLabel);

        // 折りたたみエリア
        EditorGUILayout.Space();
        showSettings = EditorGUILayout.Foldout(showSettings, "New PlayerPref Entry", true);
        if (showSettings)
        {
            ////////////////////    Add  
            // KeyTypeの選択メニュー
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            selectedValueType = (ValueType)EditorGUILayout.EnumPopup("Select Value Type", selectedValueType, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.EndHorizontal();
            // Keyの入力フィールド
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("Key:", GUILayout.Width(60));
            newKey = GUILayout.TextField(newKey, GUILayout.Width(300));

            // Keyのメッセージエリア
            GUILayout.FlexibleSpace();
            GUILayout.Label(messageKey, new GUIStyle(GUI.skin.label) { normal = { textColor = messageColorKey }, alignment = TextAnchor.MiddleCenter }, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Valueの入力フィールド
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("Value:", GUILayout.Width(60));
            string tempValue = "";

            switch (selectedValueType)
            {
                case ValueType.STRING:
                    newValue = GUILayout.TextField(newValue, GUILayout.Width(300));
                    break;
                case ValueType.INT:
                    tempValue = GUILayout.TextField(newValue, GUILayout.Width(300));
                    newValue = Regex.Replace(tempValue, "[^0-9]", ""); // 数字以外を削除
                    break;
                case ValueType.FLOAT:
                    tempValue = GUILayout.TextField(newValue, GUILayout.Width(300));
                    newValue = Regex.Replace(tempValue, "[^0-9.]", ""); // 数字と小数点以外を削除
                                                                        // 小数点が複数入力されないように制御
                    if (newValue.Count(c => c == '.') > 1)
                    {
                        int lastDotIndex = newValue.LastIndexOf('.');
                        newValue = newValue.Remove(lastDotIndex, 1);
                    }
                    break;
            }

            // Valueのメッセージエリア
            GUILayout.FlexibleSpace();
            GUILayout.Label(messageValue, new GUIStyle(GUI.skin.label) { normal = { textColor = messageColorValue }, alignment = TextAnchor.MiddleCenter }, GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // プレファレンス追加ボタン
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(360));
            if (GUILayout.Button("Add New Pref", GUILayout.Height(30)))
            {
                if (!string.IsNullOrEmpty(newKey) && !string.IsNullOrEmpty(newValue))
                {
                    if (PlayerPrefs.HasKey(newKey))
                    {
                        bool overwrite = EditorUtility.DisplayDialog("キーが既に存在します", $"キー \"{newKey}\" は既に存在します。上書きしますか？", "はい", "いいえ");
                        if (!overwrite) return;
                    }

                    bool validType = true;
                    try
                    {
                        if (selectedValueType == ValueType.INT)
                            PlayerPrefs.SetInt(newKey, int.Parse(newValue));
                        else if (selectedValueType == ValueType.FLOAT)
                            PlayerPrefs.SetFloat(newKey, float.Parse(newValue));
                        else if (selectedValueType == ValueType.STRING)
                            PlayerPrefs.SetString(newKey, newValue);
                        else
                            validType = false;
                    }
                    catch
                    {
                        validType = false;
                    }

                    if (validType)
                    {
                        newKey = "";
                        newValue = "";
                        messageKey = "✓ 正常に実行されました。";
                        messageValue = "✓ 正常に実行されました。";
                        messageColorKey = Color.green;
                        messageColorValue = Color.green;
                        LoadPlayerPrefs();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("無効なタイプ", "選択されたキータイプと入力された値が一致しません。", "OK");
                        messageKey = "選択されたキータイプと入力された値が一致しません。";
                        messageColorKey = Color.red;
                        messageColorValue = Color.red;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("無効な入力", "キーと値は空にできません！", "OK");
                    if (string.IsNullOrEmpty(newKey))
                    {
                        messageKey = "⚠ Key は空にできません！";
                        messageColorKey = Color.red;
                    }
                    else
                    {
                        messageKey = "✓ Key は正常です。";
                        messageColorKey = Color.green;
                    }
                    if (string.IsNullOrEmpty(newValue))
                    {
                        messageValue = "⚠ Value は空にできません！";
                        messageColorValue = Color.red;
                    }
                    else
                    {
                        messageValue = "✓ Value は正常です。";
                        messageColorValue = Color.green;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        // ボタンスタイルを作成
        GUIStyle deleteButtonStyle = new GUIStyle(GUI.skin.button);
        deleteButtonStyle.normal.textColor = Color.white;               // 文字色を白に設定
        deleteButtonStyle.normal.background = MakeTex(1, 1, Color.red); // 背景を赤に設定
        deleteButtonStyle.border = new RectOffset(0, 0, 0, 0);          // 四角形の角にする

        ////////////////////  再読み込み or All Clear ////////////////////////////////
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reload", GUILayout.Height(24)))
        {
            LoadPlayerPrefs();
        }
        // 全削除ボタン
        if (GUILayout.Button("Clear All", deleteButtonStyle, GUILayout.Width(200), GUILayout.Height(22)))
        {
            if (EditorUtility.DisplayDialog("警告", "本当にすべてのPlayerPrefsを削除しますか？", "はい", "いいえ"))
            {
                PlayerPrefs.DeleteAll();
                prefsKeys.Clear();
                prefsData.Clear();
            }
        }
        EditorGUILayout.EndHorizontal();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        List<string> keysToRemove = new List<string>();
        //////////////////// 表示 部分 /////////////////////////////////

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
        EditorGUILayout.EndHorizontal();



        foreach (var key in prefsKeys.Where(key => !excludedKeys.Contains(key)))
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            // スタイル設定
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            // Key（キー）を表示
            EditorGUILayout.LabelField("Key: " + key, style, GUILayout.Width(150));  // Key
            EditorGUILayout.LabelField("Type: " + GetValueTypeString(key), style, GUILayout.Width(100));  // Type

            // 詳細画面.
            if (GUILayout.Button("Open", GUILayout.Width(40)))
            {
                object value = LoadPrefValue(key);  // 値を取得
                if (value != null)
                {
                    string valueString = value.ToString();
                    ValueType keyType = GetValueType(key);
                    PrefsDetailWindow.ShowWindow(key, valueString, keyType, this);
                }
                else
                {
                    PrefsDetailWindow.ShowWindow(key, "(Empty)", ValueType.STRING, this);
                }
            }
            // Value（値）を表示
            prefsData[key] = DrawFieldForKey(key, prefsData[key]);

            // コピー用ボタン
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow"), GUILayout.Width(30)))
            {
                // コピー処理: key と値をコピーする
                string valueToCopy = prefsData[key]?.ToString() ?? "(Empty)";
                GUIUtility.systemCopyBuffer = $"{key}: {valueToCopy}";  // クリップボードにコピー

                // 通知（オプション）
                EditorUtility.DisplayDialog("コピー完了", $"{key} がクリップボードにコピーされました。", "OK");
            }
            // Saveボタン
            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                SavePrefValue(key, prefsData[key]);
            }

            // Deleteボタン
            if (GUILayout.Button("Delete",deleteButtonStyle,GUILayout.Width(60), GUILayout.Height(16)))
            {
                if (EditorUtility.DisplayDialog("削除の確認", $"本当にKey「 {key} 」を削除しますか？", "はい", "いいえ"))
                {
                    PlayerPrefs.DeleteKey(key);
                    keysToRemove.Add(key);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // 削除されたキーをリストから削除
        foreach (var key in keysToRemove)
        {
            prefsKeys.Remove(key);
            prefsData.Remove(key);
        }

  
    }

    /// <summary>
    /// キーごとのフィールドを描画する関数
    /// </summary>
    /// <param name="key">鍵</param>
    /// <param name="value">値</param>
    /// <returns> 型 の EditorGUILayout</returns>
    private object DrawFieldForKey(string key, object value)
    {
        if (value is string)
        {
            return EditorGUILayout.TextField((string)value);
        }
        else if (value is int)
        {
            return EditorGUILayout.IntField((int)value);
        }
        else if (value is float)
        {
            return EditorGUILayout.FloatField((float)value);
        }
        return null;
    }

    /// <summary>
    /// 値を保存する関数
    /// </summary>
    /// <param name="key">鍵</param>
    /// <param name="value">値</param>
    private void SavePrefValue(string key, object value)
    {
        if (value is string)
        {
            PlayerPrefs.SetString(key, (string)value);
        }
        else if (value is int)
        {
            PlayerPrefs.SetInt(key, (int)value);
        }
        else if (value is float)
        {
            PlayerPrefs.SetFloat(key, (float)value);
        }
        PlayerPrefs.Save();
    }
    /// <summary>
    /// 値の型種類 
    /// </summary>
    /// <param name="key">鍵</param>
    /// <returns><see cref="ValueType"/> 型</returns>
    private ValueType GetValueType(string key)
    {
        // prefsData の型に基づいて処理
        if (prefsData.TryGetValue(key, out var value))
        {
            if (value is int intValue)
            {
                return ValueType.INT;
            }
            else if (value is float floatValue)
            {
                return ValueType.FLOAT;
            }
            else if (value is string stringValue)
            {
                return ValueType.STRING;
            }
        }
        return ValueType.STRING;  // デフォルトはSTRING
    }
    /// <summary>
    /// 値の型種類 (文字)
    /// </summary>
    /// <param name="key"></param>
    /// <returns> string の int などなど</returns>
    private string GetValueTypeString(string key)
    {
        string valueType = "Unknown";

        // prefsData の型に基づいて処理
        if (prefsData.TryGetValue(key, out var value))
        {
            if (value is int)
            {
                return valueType = "int";
            }
            else if (value is float)
            {
                return valueType = "float";
            }
            else if (value is string)
            {
                return valueType = "string";
            }
        }
        return valueType;
    }

    /// <summary>
    /// 背景用のテクスチャを作成.
    /// </summary>
    /// <param name="width">大きさwidth</param>
    /// <param name="height">大きさheight</param>
    /// <param name="color">色</param>
    /// <returns> 作成された背景</returns>
    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = color;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
#endif