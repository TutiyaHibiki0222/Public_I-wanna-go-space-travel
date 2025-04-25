using Common;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Save
{
    using GameMenu;
    [Serializable, Tooltip("セーブデータList")]
    public class SaveList {
        [Tooltip("Listデータ")] public List<SaveConfig> saveConfigs = new List<SaveConfig>();
        public static SaveList InitLoad()
        {
            SaveList saveList = new SaveList();
            for (int i = 0; i < Config.saveMax; i++)
            {
                SaveConfig saveConfig = new SaveConfig();
                saveConfig.listNo = i;
                saveList.saveConfigs.Add(saveConfig);
            }

            return saveList;
        }
        public SaveConfig GetListSaveConfig(int _index)
        {
            if(0 <= _index && _index < saveConfigs.Count)
            {
                return saveConfigs[_index];
            }
            Debug.LogError($"配列以外です {_index} : null");
            return null;
        }
        public void Update(SaveConfig save)
        {
            saveConfigs[save.listNo] = save;
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        public static SaveList FromJson(string json)
        {
            return JsonUtility.FromJson<SaveList>(json);
        }
    }
    [Serializable,Tooltip("セーブデータ")]
    public class SaveConfig 
    {
        [Tooltip("List番号")]     public int  listNo;
        [Tooltip("開始時間")]     public float time;
        [Tooltip("死亡回数")]     public int deleteCount;
        [Tooltip("GameData")]     public string gameData;
        [Tooltip("Game難易度")]   public GameMode gameMode;

        [Tooltip("ゲームクリア")] public bool gameComplete;
        public SaveConfig()
        {
            listNo       = 0;
            time         = 0;
            deleteCount  = 0;
            gameData     = "";
            gameComplete = false;
            gameMode     = GameMode.None;
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static SaveConfig FromJson(string json)
        {
            return JsonUtility.FromJson<SaveConfig>(json);
        }

        public string CreateTextTimer()
        {
            TimeSpan timer = TimeSpan.FromSeconds(time);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", timer.Hours, timer.Minutes, timer.Seconds);
        }

    }
    [Serializable,Tooltip("ゲームデータ")]
    public class GameData
    {
        [Tooltip("プレイヤーの座標")] public Vector3 position;
        [Tooltip("保存されたマップ")] public string mapData;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static GameData FromJson(string json)
        {
            return JsonUtility.FromJson<GameData>(json);
        }
    }
}

namespace Game
{
    public enum GameState 
    { 
        GAME_LOAD,      // 読み込み時間.
        GAME_INIT,
        GAME_START,     // ゲーム開始.
        GAME_STOP,      // ポーズ中.
        GAME_OVER,      // ゲームオーバー.
        GAME_READY,     

        GAME_END        // ゲーム終了.
    }
}

namespace Common
{
    public class Config 
    {
        [Tooltip("Listの最大数")]  public const int saveMax = 3;
        [Tooltip("Listの保管Key")] public const string saveKey = "xazdha2";
    }

}

namespace GameMenu
{
    [Flags]
    public enum GameMode 
    {
    [Tooltip("何も選択されていない状態")]   None    = 0,
    [Tooltip("0001")]                       EASY    = 1 << 0,
    [Tooltip("0010")]                       NORMAL  = 1 << 1,      
    [Tooltip("0100")]                       HARD    = 1 << 2,     
    [Tooltip("1000")]                       VERYHARD = 1 << 3,

    [Tooltip("全て")]                       ALL = EASY | NORMAL | HARD | VERYHARD
    }


    public enum Menu
    {
        RESUME_GAME,
        RETURN_TO_LOBBY,
        

        EXIT_GAME_END,
    }
}

