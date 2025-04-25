using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Save;
using System;
using GameMenu;
using System.Runtime.ConstrainedExecution;
using UnityEngine.SceneManagement;



[Serializable]
public class Ber 
{
    [SerializeField, Tooltip("選択Obj")]      public GameObject gameObject;
    [SerializeField, Tooltip("ゲームモード")] public Menu menu;
    public Animator anim;
    public void Init()
    {
        if(!anim) anim = gameObject.GetComponent<Animator>();
    }
}



public class GameMenuUI : MonoBehaviour
{
    [SerializeField, Tooltip("死亡Count text")] TMP_Text deleteCountText;
    [SerializeField, Tooltip("play時間 text")] TMP_Text playTimeText;
    [SerializeField, Tooltip("選択GameObj")] List<Ber> bers;

    Coroutine currentCoroutine;
    int index = 0;

    void UpdateText()
    {
        SaveConfig s = SaveManager.Instance.openSaveConfig;
        deleteCountText.text = "DeleteCount: " + s.deleteCount.ToString();
        playTimeText.text = "PlayTime: " + s.CreateTextTimer();
    }

    private void Start()
    {
        index = 0;
        foreach (var ber in bers)
        {
            ber.Init();
        }
        //gameObject.SetActive(false);
    }

    void MoveMenu()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            bers[index].anim.SetBool("isActive", false);
            index = (index - 1 + bers.Count) % bers.Count;
            bers[index].anim.SetBool("isActive", true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            bers[index].anim.SetBool("isActive", false);
            index = (index + 1) % bers.Count;
            bers[index].anim.SetBool("isActive", true);
        }
    }

    void MenuEvent()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            switch (bers[index].menu)
            {
                case Menu.RESUME_GAME:
                    FindObjectOfType<GameManager>().SetPose(false);
                    break;
                case Menu.RETURN_TO_LOBBY:
                    SaveManager.Instance.Save();
                    SceneManager.LoadScene("Lobby");
                    break;
                case Menu.EXIT_GAME_END:
#if UNITY_EDITOR
                    Debug.Log("ゲーム終了（エディタでは動作しません）");
                    UnityEditor.EditorApplication.isPlaying = false;
#else
    // 実行中のアプリケーションを終了
    Application.Quit();
#endif
                    break;
            }
        }
    }

    private void Update()
    {
        MoveMenu();
        MenuEvent();
    }

    private void OnEnable()
    {
        index = 0;
        if (bers.Count != 0)
        {
            for (int i = 0; i < bers.Count; i++)
            {
                bers[i].anim.SetBool("isActive", false);
            }
            bers[index].anim.SetBool("isActive", true);
        }
        UpdateText();
    }
}
