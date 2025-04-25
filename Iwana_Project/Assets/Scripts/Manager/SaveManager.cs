using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Save;
using Common;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [SerializeField, Tooltip("保存リスト")] public SaveList saves;
    [Tooltip("exe終了するまで残り続ける")] public static SaveManager Instance;

    [SerializeField, Tooltip("開かれているデータ")] public SaveConfig openSaveConfig;
    [SerializeField, Tooltip("前のルームの座標")] public Dictionary<string, Vector3> keyValuePairs;
    void LoadSaveList()
    {
        // PlayerPrefsに存在しないなら初期化.
        if (!PlayerPrefs.HasKey(Config.saveKey))
        {
            saves = SaveList.InitLoad();
            print(saves.ToJson());
            PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
        }
        else
        {
            // 存在する場合.
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
    /// セーブデータを削除.
    /// </summary>
    /// <param name="index"></param>
    public void DeleteSave(int index)
    {
        saves.saveConfigs[index] = new SaveConfig();
        saves.saveConfigs[index].listNo = index;
        PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
    }
    /// <summary>
    /// セーブをする_スクショも保存.
    /// </summary>
    public void ScreenShot_OR_Save()
    {
        PlayerPrefs.SetString(Config.saveKey, saves.ToJson());
        string file = Path.Combine(Application.persistentDataPath, "save_screenshot" + openSaveConfig.listNo + ".png");
        ScreenCapture.CaptureScreenshot(file);  
    }
    /// <summary>
    /// セーブのみ.
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
