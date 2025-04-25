using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlatforCollision : MonoBehaviour
{
    [SerializeField,Tooltip("è∞ÇÃîªíË")] GameObject platform;
    public bool allowForDropThrough = false;

    private bool canDropThrough = false;
    
    private int GroundLayerIndex;
    private int IgnorePlayerLayerIndex;

    public float SpriteHeight = 0.16f;

    private float GuySpriteHeight;
    private float GuyOffsetX;
    private float GuyOffsetY;
    
    void Start()
    {
        StartCoroutine(EI_Start());
    }
    
    IEnumerator EI_Start()
    {
        yield return new WaitForEndOfFrame();

        GroundLayerIndex = LayerMask.NameToLayer("SolidObjects");
        IgnorePlayerLayerIndex = LayerMask.NameToLayer("IgnorePlayer");
    
        if(GameObject.FindWithTag("Player").GetComponent<BoxCollider2D>() != null) 
        {
            BoxCollider2D pOly = GameObject.FindWithTag("Player").GetComponent<BoxCollider2D>();

            GuySpriteHeight = pOly.bounds.size.y;

            GuyOffsetY = pOly.offset.y;

        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerCharacter2DBase>() != null)
        {
            PlayerCharacter2DBase GuysScript = other.gameObject.GetComponent<PlayerCharacter2DBase>();
            BoxCollider2D pOly = other.gameObject.GetComponent<BoxCollider2D>();
            GuySpriteHeight = pOly.bounds.size.y;
            GuyOffsetY = pOly.offset.y;

            if ((GuysScript.transform.position.y - GuySpriteHeight / 2 + GuyOffsetY + 0.00f) > platform.transform.position.y + SpriteHeight / 2) // print (" I AM ABOVE");
            {
                platform.layer = GroundLayerIndex;
                GuysScript.IsPixelMove = true;
                GuysScript.StopHorizontalMovement();
            }

            if ((GuysScript.transform.position.y - GuySpriteHeight / 2 + GuyOffsetY <= platform.transform.position.y + SpriteHeight / 2) || (canDropThrough && allowForDropThrough)) //print (" I AM BELOW");
            {
                platform.layer = IgnorePlayerLayerIndex;
                GuysScript.RefreshMultiJump();

                canDropThrough = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerCharacter2DBase>() != null)
        {
            other.gameObject.GetComponent<PlayerCharacter2DBase>().IsPixelMove = false;
            platform.layer = GroundLayerIndex;
        }
    }


}
