using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Save;
using Common;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [SerializeField, Tooltip("�ۑ����X�g")] public SaveList saves;
    [Tooltip("exe�I������܂Ŏc�葱����")] public static SaveManager Instance;

    [SerializeField, Tooltip("�J����Ă���f�[�^")] public SaveConfig openSaveConfig;
    [SerializeField, Tooltip("�O�̃��[���̍��W")] public Dictionary<string, Vector3> keyValuePairs;
    void LoadSaveList()
    {
        // PlayerPrefs�ɑ��݂��Ȃ��Ȃ珉����.
        if (!PlayerPrefs.HasKey(Config.saveKey))
        {
            saves = SaveList.InitLoad();
            print(saves.ToJson());
            PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
        }
        else
        {
            // ���݂���ꍇ.
            saves = SaveList.FromJson(PlayerPrefs.GetString(Config.saveKey));
        }
    }

    public void OpenSave(int index)
    {
        openSaveConfig = saves.GetListSaveConfig(index);
        if (openSaveConfig == null)
        {
            Debug.LogError("openSaveConfig : null");
            return;
        }
        if(openSaveConfig.gameData.Length <= 0)
        {
            SceneManager.LoadScene("Tutorial-1");
        }
        else
        {
            GameData gameSave = GameData.FromJson(openSaveConfig.gameData);
            SceneManager.LoadScene(gameSave.mapData);
        }
    }
    /// <summary>
    /// �Z�[�u�f�[�^���폜.
    /// </summary>
    /// <param name="index"></param>
    public void DeleteSave(int index)
    {
        saves.saveConfigs[index] = new SaveConfig();
        saves.saveConfigs[index].listNo = index;
        PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
    }
    /// <summary>
    /// �Z�[�u������_�X�N�V�����ۑ�.
    /// </summary>
    public void ScreenShot_OR_Save()
    {
        PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
        string file = Path.Combine(Application.persistentDataPath, "save_screenshot" + openSaveConfig.listNo + ".png");
        ScreenCapture.CaptureScreenshot(file);  
    }
    /// <summary>
    /// �Z�[�u�̂�.
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
    }

    // Start is called before the first frame update
    void Awake()
    {
        keyValuePairs = new Dictionary<string, Vector3>();
        openSaveConfig = null;
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSaveList();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
        Instance = null;
        Destroy(gameObject);
    }
}
