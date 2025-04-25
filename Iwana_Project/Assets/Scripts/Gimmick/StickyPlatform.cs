using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == ("Player") && other.GetComponent<PlayerCharacter2DBase>())
        {
            other.gameObject.transform.parent = this.transform;
            other.GetComponent<PlayerCharacter2DBase>().IsPixelMove = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == ("Player") && other.transform.parent == this.transform && other.GetComponent<PlayerCharacter2DBase>())
        {
            other.gameObject.transform.parent = null;

            other.GetComponent<PlayerCharacter2DBase>().RefreshMultiJump();
            other.GetComponent<PlayerCharacter2DBase>().IsPixelMove = false;
        }
    }
}
