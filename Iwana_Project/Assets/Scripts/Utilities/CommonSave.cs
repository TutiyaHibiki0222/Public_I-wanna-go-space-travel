using Common;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Save
{
    using GameMenu;
    [Serializable, Tooltip("�Z�[�u�f�[�^List")]
    public class SaveList {
        [Tooltip("List�f�[�^")] public List<SaveConfig> saveConfigs = new List<SaveConfig>();
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
            Debug.LogError($"�z��ȊO�ł� {_index} : null");
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
    [Serializable,Tooltip("�Z�[�u�f�[�^")]
    public class SaveConfig 
    {
        [Tooltip("List�ԍ�")]     public int  listNo;
        [Tooltip("�J�n����")]     public float time;
        [Tooltip("���S��")]     public int deleteCount;
        [Tooltip("GameData")]     public string gameData;
        [Tooltip("Game��Փx")]   public GameMode gameMode;

        [Tooltip("�Q�[���N���A")] public bool gameComplete;
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
    [Serializable,Tooltip("�Q�[���f�[�^")]
    public class GameData
    {
        [Tooltip("�v���C���[�̍��W")] public Vector3 position;
        [Tooltip("�ۑ����ꂽ�}�b�v")] public string mapData;

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
        GAME_LOAD,      // �ǂݍ��ݎ���.
        GAME_INIT,
        GAME_START,     // �Q�[���J�n.
        GAME_STOP,      // �|�[�Y��.
        GAME_OVER,      // �Q�[���I�[�o�[.
        GAME_READY,     

        GAME_END        // �Q�[���I��.
    }
}

namespace Common
{
    public class Config 
    {
        [Tooltip("List�̍ő吔")]  public const int saveMax = 3;
        [Tooltip("List�̕ۊ�Key")] public const string saveKey = "xazdha2";
    }

}

namespace GameMenu
{
    [Flags]
    public enum GameMode 
    {
    [Tooltip("�����I������Ă��Ȃ����")]   None    = 0,
    [Tooltip("0001")]                       EASY    = 1 << 0,
    [Tooltip("0010")]                       NORMAL  = 1 << 1,      
    [Tooltip("0100")]                       HARD    = 1 << 2,     
    [Tooltip("1000")]                       VERYHARD = 1 << 3,

    [Tooltip("�S��")]                       ALL = EASY | NORMAL | HARD | VERYHARD
    }


    public enum Menu
    {
        RESUME_GAME,
        RETURN_TO_LOBBY,
        

        EXIT_GAME_END,
    }
}

