using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SaveDataList : MonoBehaviour
{
    [SerializeField,Tooltip("ƒŠƒXƒg")]  List<SavePanel> panelList = new List<SavePanel>();
    SaveManager saveMgr;
    RectTransform rectTransform;

    public int index;

    void Init()
    {
        panelList = new List<SavePanel>();
        if (!rectTransform) rectTransform = GetComponent<RectTransform>();
        if(!saveMgr) saveMgr = SaveManager.Instance;
        var getList = saveMgr.saves.saveConfigs;
        for (int i = 0; i < getList.Count; i++)
        {
            var obj = transform.GetChild(i).gameObject;
            if (obj)
            {
                var sp = obj.GetComponent<SavePanel>();
                panelList.Add(sp);
                var save = saveMgr.saves.saveConfigs[i];
                sp.SetFilm(false);sp.SetSaveConfig(save);
            }
        }

        panelList[index].SetFilm(true);
        foreach(var panel in panelList)
        {
            panel.Init();
        }

    }


    void ChangeSaveList()
    {
        if      (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            panelList[index].SetFilm(false);
            index--;
            if (index < 0)
            {
                index = panelList.Count - 1;
            }
            panelList[index].SetFilm(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            panelList[index].SetFilm(false);
            index++;
            if (index > panelList.Count - 1)
            {
                index = 0;
            }
            panelList[index].SetFilm(true);
        }
    }

    void DecisionSaveOpen()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            saveMgr.OpenSave(index);
        }
    }

    void DeleteSave()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            saveMgr.DeleteSave(index);
            panelList[index].DeleteSave();
            Init();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSaveList();
        DecisionSaveOpen();
        DeleteSave();
    }
}
