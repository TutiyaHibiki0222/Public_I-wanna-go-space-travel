using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Microsoft.Win32;

public static class PlayerPrefsTools
{
    public static void GetAllPlayerPrefKeys(ref List<string> keys)
    {
        // キーリストがnullの場合、初期化する
        keys ??= new List<string>();

        // リストをクリアする
        keys.Clear();

#if UNITY_STANDALONE_WIN
        try
        {
            string regKeyPathPattern =
#if UNITY_EDITOR
                @"Software\Unity\UnityEditor\{0}\{1}"; // Unityエディタ用
#else
                @"Software\{0}\{1}"; // ビルド版用
#endif

            string regKeyPath = string.Format(regKeyPathPattern, PlayerSettings.companyName, PlayerSettings.productName);
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(regKeyPath))
            {
                if (regKey == null) return;

                // レジストリのキーを取得し、_hXXXXXサフィックスを削除してキーをリストに追加
                foreach (string playerPrefKey in regKey.GetValueNames())
                {
                    // playerPrefKey を変更したい場合は、別の変数に代入
                    string modifiedKey = playerPrefKey;
                    int suffixIndex = modifiedKey.LastIndexOf("_h");
                    if (suffixIndex > 0)
                    {
                        modifiedKey = modifiedKey.Substring(0, suffixIndex);
                    }

                    keys.Add(modifiedKey);
                }

            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error while fetching PlayerPrefs keys: {ex.Message}");
        }
#endif
    }
}
