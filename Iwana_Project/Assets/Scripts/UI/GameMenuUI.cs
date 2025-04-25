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
    [SerializeField, Tooltip("�I��Obj")]      public GameObject gameObject;
    [SerializeField, Tooltip("�Q�[�����[�h")] public Menu menu;
    public Animator anim;
    public void Init()
    {
        if(!anim) anim = gameObject.GetComponent<Animator>();
    }
}



public class GameMenuUI : MonoBehaviour
{
    [SerializeField, Tooltip("���SCount text")] TMP_Text deleteCountText;
    [SerializeField, Tooltip("play���� text")] TMP_Text playTimeText;
    [SerializeField, Tooltip("�I��GameObj")] List<Ber> bers;

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
                    Debug.Log("�Q�[���I���i�G�f�B�^�ł͓��삵�܂���j");
                    UnityEditor.EditorApplication.isPlaying = false;
#else
    // ���s���̃A�v���P�[�V�������I��
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
