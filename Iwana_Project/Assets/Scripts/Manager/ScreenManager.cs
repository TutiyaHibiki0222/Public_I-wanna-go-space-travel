using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField, Tooltip("�Q�[���I�[�o�[")] public GameObject gameOver;
    [SerializeField, Tooltip("�Q�[����")] GameObject posePanel;

    public GameObject GetPose {  get { return posePanel; } }

    private void Awake()
    {
        gameOver  = transform.Find("GameOver").gameObject;
        posePanel = transform.Find("PausePanel").gameObject;
    }
}
