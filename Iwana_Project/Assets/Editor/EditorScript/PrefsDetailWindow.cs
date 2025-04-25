#if UNITY_EDITOR
/* 
    - 2025 2/04 �y�� �� ���� 
    PlayerPrefsEditor ���� �J���ꂽ PlayerPrefs��ҏW���� EditorWindow.
*/


using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using System.Text;

public class PrefsDetailWindow : EditorWindow
{
    // �e�E�B���h�E�̎Q�Ƃ�ێ�
    PlayerPrefsEditor playerPrefsEditor;

    string key;
    string initValue;
    string resetValue;
    string value;
    string newKey;
    ValueType selectedValueType;
    ValueType initselectedValueType;

    // �X�N���[���r���[�̃X�N���[���ʒu
    private Vector2 scrollPosition;
    private Vector2 previewScrollPosition;

    bool isKeyChanged;
    bool isTypeChanged;
    bool isValueInitialized = false;
    bool drawJson;

    // PrefsDetailWindow��\�����郁�\�b�h.
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

    // �ҏW��ʂ�UI���쐬
    private void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField($"Edit Prefs [{key}]", EditorStyles.boldLabel);
        // �ۑ��{�^��
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
        // Key�̕ύX�̗L����I������`�F�b�N�{�b�N�X
        isKeyChanged = EditorGUILayout.Toggle("Change Key", isKeyChanged);

        // Key�̕ҏW�t�B�[���h�i�L�[�̕ύX���\�ɂ���j
        if (isKeyChanged)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("New Key: ", GUILayout.Width(60));
            newKey = GUILayout.TextField(newKey, GUILayout.Width(300));  // �V�����L�[�̓��̓t�B�[���h
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

        // �^�̕ύX�̗L����I������`�F�b�N�{�b�N�X
        isTypeChanged = EditorGUILayout.Toggle("Change Type", isTypeChanged);

        // �^�I���̃t�B�[���h�i�^�̕ύX���\�ɂ���j
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

        // selectedValueType �� STRING �Ȃ����� Json�ɕϊ��ł��邩?
        if (selectedValueType == ValueType.STRING && IsJsonValid(value))
             drawJson = EditorGUILayout.Toggle("Draw Json", drawJson);
        else drawJson = false;

        // 
        if (drawJson)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
            // JSON�`���̃f�[�^��\��.
            try
            {
                // value��JSON�`�����Ɖ��肵�āA�f�V���A���C�Y�����`
                var parsedJson = JsonConvert.DeserializeObject(value);

                // ���`���ꂽJSON�i�C���f���g����j��\��
                var formattedJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                formattedJson = EditorGUILayout.TextArea(formattedJson);  // �����s�̓��̓G���A

                // ���[�U�[���ҏW������̐��`���ꂽJSON��ۑ�
                value = formattedJson;

                // �ۑ��O�Ɉ�s�ɂ܂Ƃ߂�
                var compactJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(value), Formatting.None);
                value = compactJson;
            }
            catch (JsonException ex)
            {
                // JSON�p�[�X�G���[�����������ꍇ
                EditorGUILayout.LabelField("Invalid JSON", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(ex.Message);
                value = EditorGUILayout.TextArea(value);
            }
        }
        else
        {
            // �X�N���[���\�ȗ̈�.
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
            // �����œ��̓G���A�̍����𐧌�����
            float maxLines = 10f;    // �ő�s��
            float lineHeight = 16f;  // �e�s�̍���
            float textAreaHeight = Mathf.Min(value.Split('\n').Length, maxLines) * lineHeight;

            // �����s�̓��̓G���A
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
        // 1�ڂ̃X�N���[���r���[���I��.
        EditorGUILayout.EndScrollView();    
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), lineColor);
        EditorGUILayout.Space();

        GUILayout.Label("Changes Preview:");
        // �ύX���e��\�����邽�߂̃X�N���[���r���[
        previewScrollPosition = EditorGUILayout.BeginScrollView(previewScrollPosition, GUILayout.Height(150));

        // Key�̕ύX������Ε\���i�j
        if (isKeyChanged)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Key: {key} -> <color=#3399FF>{newKey}</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), new Color(0.2f, 0.6f, 1.0f, 1.0f)); // ��
            GUILayout.EndVertical();
        }

        // �^�̕ύX������Ε\���i�I�����W�j
        if (isTypeChanged)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Type: {initselectedValueType} -> <color=#FF9933>{selectedValueType}</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), new Color(1.0f, 0.6f, 0.2f, 1.0f)); // �I�����W
            GUILayout.EndVertical();
        }

        // �l�̕ύX������Ε\���i�΁j
        if (value != initValue)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Value: {initValue} \n �� \n<color=#33CC33>Value: {value}</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), new Color(0.2f, 1.0f, 0.2f, 1.0f)); // ��
            GUILayout.EndVertical();
        }

        // ���ׂĂ̕ύX���Ȃ��ꍇ�Ƀf�t�H���g��Ԃ�\������ꍇ.
        if (!isKeyChanged && !isTypeChanged && value == initValue)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("No Changes", EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }
    /// �������n�C���C�g���郁�\�b�h
    public string HighlightChanges(string original, string modified)
    {
        int minLength = Mathf.Min(original.Length, modified.Length);
        StringBuilder result = new StringBuilder();

        // �������Ƃ̔�r
        for (int i = 0; i < minLength; i++)
        {
            if (original[i] == modified[i])
            {
                // �ύX�Ȃ�
                result.Append(modified[i]);
            }
            else
            {
                // �ύX���ꂽ�����̓I�����W�F
                result.Append($"<color=#FFA500><b>{modified[i]}</b></color>");
            }
        }

        // �폜���ꂽ�����i���̕�����ɂ����ĐV����������ɂȂ������j
        if (original.Length > modified.Length)
        {
            result.Append($"<color=#FF6347><b>{original.Substring(minLength)}</b></color>"); // �ԐF�ŕ\��
        }

        // �ǉ����ꂽ�����i�V����������ɂ����Č��̕�����ɂȂ������j
        if (modified.Length > original.Length)
        {
            result.Append($"<color=#32CD32><b>{modified.Substring(minLength)}</b></color>"); // �F�ŕ\��
        }

        return result.ToString();
    }
    /// <summary>
    /// PlayerPrefs �ۑ��� ����.
    /// </summary>
    private new void SaveChanges()
    {
        bool validType = true;

        // �V�����L�[�����Z�b�g
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
            // �^�Ɋ�Â��ĕۑ��������s��
            if (selectedValueType == ValueType.INT)
            {
                int intValue = int.Parse(value);

                // �Â��L�[���폜���ĐV�����L�[�ŕۑ�
                if (key != newKey) PlayerPrefs.DeleteKey(key);
                PlayerPrefs.SetInt(newKey, intValue);
            }
            else if (selectedValueType == ValueType.FLOAT)
            {
                float floatValue = float.Parse(value);

                // �Â��L�[���폜���ĐV�����L�[�ŕۑ�
                if (key != newKey) PlayerPrefs.DeleteKey(key);
                PlayerPrefs.SetFloat(newKey, floatValue);
            }
            else if (selectedValueType == ValueType.STRING)
            {
                // �Â��L�[���폜���ĐV�����L�[�ŕۑ�
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
            // �ҏW��̃L�[�ƒl���ۑ����ꂽ
            Debug.Log("Changes Saved");
            playerPrefsEditor.LoadPlayerPrefs();
            Close();
        }
        else
        {
            EditorUtility.DisplayDialog("�����ȓ���", "�I�����ꂽ�L�[�^�C�v�Ɠ��͂��ꂽ�l����v���܂���B", "OK");
        }
    }
    /// <summary>
    /// Json�ɕϊ��\��.
    /// </summary>
    /// <param name="value"> �v�f </param>
    /// <returns> �\�Ȃ� true ����ȊO false </returns>
    bool IsJsonValid(string value)
    {
        try
        {
            // JSON �Ƃ��ăp�[�X�����݂�
            JsonConvert.DeserializeObject(value);
            return true;  // �G���[���������Ȃ���΁A�L����JSON
        }
        catch (JsonException)
        {
            return false;  // JSON �`���łȂ��ꍇ�̓G���[������
        }
    }
}

#endif
