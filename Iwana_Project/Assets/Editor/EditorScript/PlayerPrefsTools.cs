using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Microsoft.Win32;

public static class PlayerPrefsTools
{
    public static void GetAllPlayerPrefKeys(ref List<string> keys)
    {
        // �L�[���X�g��null�̏ꍇ�A����������
        keys ??= new List<string>();

        // ���X�g���N���A����
        keys.Clear();

#if UNITY_STANDALONE_WIN
        try
        {
            string regKeyPathPattern =
#if UNITY_EDITOR
                @"Software\Unity\UnityEditor\{0}\{1}"; // Unity�G�f�B�^�p
#else
                @"Software\{0}\{1}"; // �r���h�ŗp
#endif

            string regKeyPath = string.Format(regKeyPathPattern, PlayerSettings.companyName, PlayerSettings.productName);
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(regKeyPath))
            {
                if (regKey == null) return;

                // ���W�X�g���̃L�[���擾���A_hXXXXX�T�t�B�b�N�X���폜���ăL�[�����X�g�ɒǉ�
                foreach (string playerPrefKey in regKey.GetValueNames())
                {
                    // playerPrefKey ��ύX�������ꍇ�́A�ʂ̕ϐ��ɑ��
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
