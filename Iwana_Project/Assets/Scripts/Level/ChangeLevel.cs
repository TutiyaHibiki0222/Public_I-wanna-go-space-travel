using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMenu;
using System;

[Serializable, Tooltip("êVÇµÇ¢ ïœçXñº")]
class ChangeConfig
{
    [SerializeField] public NextStege  nextStege;
    [SerializeField] public string newName;
}


public class ChangeLevel : MonoBehaviour
{
    GameMode mode;
    [Header("EASY")]
    [SerializeField] List<ChangeConfig> easyConfigs;
    [Header("NORMAL")]
    [SerializeField] List<ChangeConfig> normalConfigs;
    [Header("HARD")]
    [SerializeField] List<ChangeConfig> hardConfigs;
    [Header("VERYHARD")]
    [SerializeField] List<ChangeConfig> veryhadeConfigs;
    private void Awake()
    {
        if (SaveManager.Instance)
        {
            if (SaveManager.Instance.openSaveConfig != null)
                mode = SaveManager.Instance.openSaveConfig.gameMode;
        }
        else
        {
            mode = GameMode.None;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        switch (mode)
        {
            case GameMode.EASY:
                ChengNextStege(easyConfigs);
                break;
            case GameMode.NORMAL:
                ChengNextStege(normalConfigs);
                break;
            case GameMode.HARD:
                ChengNextStege(hardConfigs);
                break;
            case GameMode.VERYHARD:
                ChengNextStege(veryhadeConfigs);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// ChangeConfig Listì‡ÇÃóvëfÇèàóù.
    /// </summary>
    /// <param name="datas"></param>
    void ChengNextStege(List<ChangeConfig> datas)
    {
        foreach (ChangeConfig c in datas)
        {
           if(c.nextStege != null)
           {
                c.nextStege.sceneName = c.newName;
           }
        }
    }
}
