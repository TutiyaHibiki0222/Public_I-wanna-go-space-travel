using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signboard : MonoBehaviour
{
    public GameObject messagePrefab;
    public float ChangeDelay = 3;

    public string targetTag = "Player";
    public bool isLoopText = false;
    [TextArea] public string[] messages;

    GameObject message;

    void Init()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == targetTag)
        {
            StartCoroutine(ChangeSwapping());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == targetTag)
        {
            StopAllCoroutines();
            if (message != null)
            {
                Destroy(message);
            }
        }
    }

    IEnumerator ChangeSwapping()
    {
        for (int i = 0; i < messages.Length; i++)
        {
            NewMessage();
            message.GetComponentInChildren<Text>().text = messages[i];

            yield return new WaitForSeconds(ChangeDelay);
        }

        if (isLoopText)
            StartCoroutine(ChangeSwapping());
        else
            message.SetActive(false);
    }

    void NewMessage()
    {
        if (message != null)
            return;
        message = Instantiate(messagePrefab);
        message.transform.SetParent(transform, false);
    }
}
