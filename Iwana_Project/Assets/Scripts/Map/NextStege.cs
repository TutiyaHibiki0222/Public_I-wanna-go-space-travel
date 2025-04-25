using Attribute.Add;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStege : MonoBehaviour
{
    [SerializeField, Tooltip("���̃X�e�[�W")] public string sceneName;

    [Tooltip("�����ԍ�"),SerializeField]        bool SponeSet;
    [ShowIf("SponeSet"), SerializeField,Min(0)] public int SponeNo = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (SponeSet)
                PlayerPrefs.SetInt("nextIndex", SponeNo);
            else
                PlayerPrefs.SetInt("nextIndex", 0);

            SceneManager.LoadScene(sceneName);
        }
        
    }
}
