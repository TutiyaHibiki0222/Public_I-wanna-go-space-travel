using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField, Tooltip("ゲームオーバー")] public GameObject gameOver;
    [SerializeField, Tooltip("ゲーム状況")] GameObject posePanel;

    public GameObject GetPose {  get { return posePanel; } }

    private void Awake()
    {
        gameOver  = transform.Find("GameOver").gameObject;
        posePanel = transform.Find("PausePanel").gameObject;
    }
}
