using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenBulletDies : MonoBehaviour
{
    public float deathDelay;

    private void OnEnable()
    {
        StartCoroutine(DisableAfter(deathDelay));
    }

    IEnumerator DisableAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
}
