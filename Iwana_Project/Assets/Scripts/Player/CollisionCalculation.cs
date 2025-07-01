using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInfo
{
    public float ColliderIndent = 0.009f;

    // Public properties for calculated distances
    public float DistanceToGround   { get; set; } = 1;
    public float DistanceToWall     { get; set; } = 1;
    public float DistanceToCeiling  { get; set; } = 1;
    public float DistanceToPit      { get; set; } = 1;

    // Collider properties
    public float ColliderWidth      { get; private set; }
    public float ColliderHeight     { get; private set; }
    public float ColliderOffsetX    { get; private set; }
    public float ColliderOffsetY    { get; private set; }

    public CollisionInfo(GameObject game)
    {
        // Shape of the players collider
        BoxCollider2D poly  = game.GetComponent<BoxCollider2D>();
        ColliderWidth       = poly.bounds.size.x;
        ColliderHeight      = poly.bounds.size.y;
        ColliderOffsetX     = poly.offset.x * Mathf.Abs(game.transform.localScale.x);
        ColliderOffsetY     = poly.offset.y * Mathf.Abs(game.transform.localScale.y);
    }
}


public class CollisionCalculation : MonoBehaviour
{
    [SerializeField, Tooltip("Ç«ÇÃÉåÉCÉÑÅ[è∞Ç∆ÇµÇƒîªíË")] LayerMask whatIsGround;
    public CollisionInfo colInfo;

    private PlayerCharacter2DBase player;

    private float colliderIndent = 0.009f;
    private bool isBackwardsSpriteX = false;

    private Vector2 TopLeft;
    private Vector2 BottomRight;

    public void Init()
    {
        colInfo = new CollisionInfo(gameObject);

        if (GetComponent<PlayerCharacter2DBase>())
            player = GetComponent<PlayerCharacter2DBase>();
    }
    public void ChangeLayerMask(LayerMask newLayerMask)
    {
        whatIsGround = newLayerMask;
    }
    public void RecheckThisObjectsStats()
    {
        colInfo = new CollisionInfo(gameObject);
    }
    public void SetColliderIndent(float new_Indent)
    {
        colliderIndent = new_Indent;
    }
    public void SetBackwardsSprite_X(bool flipit)
    {
        isBackwardsSpriteX = flipit;
    }

    public float GroundCheck(bool isUpright)
    {
        Collider2D GroundObject = null;

        float X_Flip = gameObject.transform.localScale.x / Mathf.Abs(gameObject.transform.localScale.x);
        float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs(gameObject.transform.localScale.y);

        TopLeft.x = gameObject.transform.position.x - colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip + colInfo.ColliderIndent;
        TopLeft.y = gameObject.transform.position.y - colInfo.ColliderHeight / 2 * Y_Flip + colInfo.ColliderOffsetY * Y_Flip + 0.05f * Y_Flip;
        BottomRight.x = gameObject.transform.position.x + colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip - colInfo.ColliderIndent;
        BottomRight.y = gameObject.transform.position.y - colInfo.ColliderHeight / 2 * Y_Flip + colInfo.ColliderOffsetY * Y_Flip - 0.30f * Y_Flip;

        GroundObject = Physics2D.OverlapArea(TopLeft, BottomRight, whatIsGround);

        if (GroundObject != null)
        {
            if (GroundObject.GetComponent<BoxCollider2D>() && GroundObject.transform.rotation == Quaternion.identity)
            {
                float objectOffset = 0;
                objectOffset = GroundObject.GetComponent<BoxCollider2D>().offset.y;
                objectOffset *= GroundObject.transform.localScale.y;

                colInfo.DistanceToGround = (Mathf.Abs(gameObject.transform.position.y - GroundObject.gameObject.transform.position.y + colInfo.ColliderOffsetY * Y_Flip - objectOffset) - colInfo.ColliderHeight / 2 - GroundObject.bounds.size.y / 2);

                if (player)
                    player.AllowPreCheckGround = true;
            }
            else
            {
                Vector3 TEMP    = transform.position + new Vector3(colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip - colInfo.ColliderIndent, colInfo.ColliderOffsetY * Y_Flip, 0);
                Vector3 TEMP2   = transform.position + new Vector3(-colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip + colInfo.ColliderIndent, colInfo.ColliderOffsetY * Y_Flip, 0);

                colInfo.DistanceToGround = 1;
                float DistanceToGround2 = 1;

                RaycastHit2D hit = Physics2D.Raycast(TEMP, -Vector2.up * Y_Flip, 0.5f, whatIsGround);
                RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, -Vector2.up * Y_Flip, 0.5f, whatIsGround);

                if (hit.collider != null)
                {
                    colInfo.DistanceToGround = (Mathf.Abs(hit.point.y - transform.position.y - colInfo.ColliderOffsetY * Y_Flip) - colInfo.ColliderHeight / 2);
                }
                if (hit2.collider != null)
                {
                    DistanceToGround2 = (Mathf.Abs(hit2.point.y - transform.position.y - colInfo.ColliderOffsetY * Y_Flip) - colInfo.ColliderHeight / 2);
                }

                if (Mathf.Abs(colInfo.DistanceToGround) > Mathf.Abs(DistanceToGround2))
                    colInfo.DistanceToGround = DistanceToGround2;

                if (player)
                    player.AllowPreCheckGround = false;
            }
        }
        else
        {
            colInfo.DistanceToGround = 1;
        }

        return colInfo.DistanceToGround;
    }
    public float CeilingCheck(bool isUpright)
    {
        Collider2D CeilingObject = null;

        float X_Flip = gameObject.transform.localScale.x / Mathf.Abs(gameObject.transform.localScale.x);
        float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs(gameObject.transform.localScale.y);

        TopLeft.x = gameObject.transform.position.x - colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip + colInfo.ColliderIndent;
        TopLeft.y = gameObject.transform.position.y + colInfo.ColliderHeight / 2 * Y_Flip + colInfo.ColliderOffsetY * Y_Flip - 0.02f * Y_Flip;
        BottomRight.x = gameObject.transform.position.x + colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip - colInfo.ColliderIndent;
        BottomRight.y = gameObject.transform.position.y + colInfo.ColliderHeight / 2 * Y_Flip + colInfo.ColliderOffsetY * Y_Flip + 0.15f * Y_Flip;

        CeilingObject = Physics2D.OverlapArea(TopLeft, BottomRight, whatIsGround);

        if (CeilingObject != null)
        {
            if (CeilingObject.GetComponent<BoxCollider2D>() && CeilingObject.transform.rotation == Quaternion.identity)
            {
                float objectOffset = 0;
                objectOffset = CeilingObject.GetComponent<BoxCollider2D>().offset.y;
                objectOffset *= CeilingObject.transform.localScale.y;

                colInfo.DistanceToCeiling = (Mathf.Abs(gameObject.transform.position.y - CeilingObject.gameObject.transform.position.y + colInfo.ColliderOffsetY * Y_Flip - objectOffset) - colInfo.ColliderHeight / 2 - CeilingObject.bounds.size.y / 2);

                if (player)
                    player.AllowPreCheckCeiling = true;
            }
            else
            {
                Vector3 TEMP = transform.position + new Vector3(colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip - colInfo.ColliderIndent, colInfo.ColliderOffsetY * Y_Flip, 0);
                Vector3 TEMP2 = transform.position + new Vector3(-colInfo.ColliderWidth / 2 + colInfo.ColliderOffsetX * X_Flip + colInfo.ColliderIndent, colInfo.ColliderOffsetY * Y_Flip, 0);

                colInfo.DistanceToCeiling = 1;
                float DistanceToCeiling2 = 1;

                RaycastHit2D hit = Physics2D.Raycast(TEMP, Vector2.up * Y_Flip, 0.5f, whatIsGround);
                RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, Vector2.up * Y_Flip, 0.5f, whatIsGround);

                if (hit.collider != null)
                {
                    colInfo.DistanceToCeiling = (Mathf.Abs(hit.point.y - transform.position.y - colInfo.ColliderOffsetY * Y_Flip) - colInfo.ColliderHeight / 2);
                }
                if (hit2.collider != null)
                {
                    DistanceToCeiling2 = (Mathf.Abs(hit2.point.y - transform.position.y - colInfo.ColliderOffsetY * Y_Flip) - colInfo.ColliderHeight / 2);
                }

                if (Mathf.Abs(colInfo.DistanceToCeiling) > Mathf.Abs(DistanceToCeiling2))
                    colInfo.DistanceToCeiling = DistanceToCeiling2;

                if (player)
                    player.AllowPreCheckCeiling = false;
            }
        }
        else
        {
            colInfo.DistanceToCeiling = 1;
        }

        return colInfo.DistanceToCeiling;
    }
    public float WallCheck(bool isUpright, bool facingRight)
    {
        Collider2D WallObject = null;

        float X_Flip = gameObject.transform.localScale.x / Mathf.Abs(gameObject.transform.localScale.x);
        float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs(gameObject.transform.localScale.y);

        if (isBackwardsSpriteX)
            X_Flip *= -1;

        TopLeft.x = gameObject.transform.position.x + colInfo.ColliderWidth / 2 * X_Flip + colInfo.ColliderOffsetX * X_Flip - 0.05f * X_Flip;
        TopLeft.y = gameObject.transform.position.y + colInfo.ColliderHeight / 2 + colInfo.ColliderOffsetY * Y_Flip - colInfo.ColliderIndent;
        BottomRight.x = gameObject.transform.position.x + colInfo.ColliderWidth / 2 * X_Flip + colInfo.ColliderOffsetX * X_Flip + 0.10f * X_Flip;
        BottomRight.y = gameObject.transform.position.y - colInfo.ColliderHeight / 2 + colInfo.ColliderOffsetY * Y_Flip + colInfo.ColliderIndent;
        

        WallObject = Physics2D.OverlapArea(TopLeft, BottomRight, whatIsGround);
        if (WallObject != null)
        {
            if (WallObject.GetComponent<BoxCollider2D>() && WallObject.transform.rotation == Quaternion.identity)
            {
                float objectOffset = 0;
                objectOffset = WallObject.GetComponent<BoxCollider2D>().offset.x;
                objectOffset *= WallObject.transform.localScale.x;

                colInfo.DistanceToWall = (Mathf.Abs(gameObject.transform.position.x - WallObject.gameObject.transform.position.x + colInfo.ColliderOffsetX * X_Flip - objectOffset) - colInfo.ColliderWidth / 2 - WallObject.bounds.size.x / 2);

                if (player)
                    player.AllowPreCheckWall = true;
            }
            else
            {
                Vector3 TEMP = transform.position + new Vector3(colInfo.ColliderOffsetX * X_Flip, colInfo.ColliderHeight / 2 + colInfo.ColliderOffsetY * Y_Flip - colInfo.ColliderIndent, 0);
                Vector3 TEMP2 = transform.position + new Vector3(colInfo.ColliderOffsetX * X_Flip, -colInfo.ColliderHeight / 2 + colInfo.ColliderOffsetY * Y_Flip + colInfo.ColliderIndent, 0);

                colInfo.DistanceToWall = 1;
                float DistanceToWall2 = 1;

                RaycastHit2D hit  = Physics2D.Raycast(TEMP,  Vector2.right * X_Flip, 0.5f, whatIsGround);
                RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, Vector2.right * X_Flip, 0.5f, whatIsGround);

                if (hit.collider != null)
                {
                    colInfo.DistanceToWall = (Mathf.Abs(hit.point.x - transform.position.x - colInfo.ColliderOffsetX * X_Flip) - colInfo.ColliderWidth / 2);
                }
                if (hit2.collider != null)
                {
                    DistanceToWall2 = (Mathf.Abs(hit2.point.x - transform.position.x - colInfo.ColliderOffsetX * X_Flip) - colInfo.ColliderWidth / 2);
                }

                if (Mathf.Abs(colInfo.DistanceToWall) > Mathf.Abs(DistanceToWall2))
                    colInfo.DistanceToWall = DistanceToWall2;

                if (player)
                    player.AllowPreCheckWall = false;
            }
        }
        else
        {
            colInfo.DistanceToWall = 1;
        }

        return colInfo.DistanceToWall;
    }

    public float PitCheck(bool isUpright, bool facingRight)
    {
        Collider2D PitObject = null;

        float X_Flip = gameObject.transform.localScale.x / Mathf.Abs(gameObject.transform.localScale.x);
        float Y_Flip = gameObject.transform.localScale.y / Mathf.Abs(gameObject.transform.localScale.y);

        if (isBackwardsSpriteX)
            X_Flip *= -1;

        TopLeft.x = gameObject.transform.position.x + colInfo.ColliderWidth / 2 * X_Flip + colInfo.ColliderOffsetX * X_Flip;
        TopLeft.y = gameObject.transform.position.y - colInfo.ColliderHeight / 2 * Y_Flip + colInfo.ColliderOffsetY * Y_Flip + 0.01f * Y_Flip;
        BottomRight.x = gameObject.transform.position.x + colInfo.ColliderWidth * X_Flip + colInfo.ColliderOffsetX * X_Flip;
        BottomRight.y = gameObject.transform.position.y - colInfo.ColliderHeight / 2 * Y_Flip + colInfo.ColliderOffsetY * Y_Flip - 0.16f * Y_Flip;

        PitObject = Physics2D.OverlapArea(TopLeft, BottomRight, whatIsGround);

        if (PitObject != null)
        {
            if (PitObject.GetComponent<BoxCollider2D>() && PitObject.transform.rotation == Quaternion.identity)
            {
                float objectOffset = 0;
                objectOffset = PitObject.GetComponent<BoxCollider2D>().offset.y;
                objectOffset *= PitObject.transform.localScale.y;

                colInfo.DistanceToPit = (Mathf.Abs(gameObject.transform.position.y - PitObject.gameObject.transform.position.y + colInfo.ColliderOffsetY * Y_Flip - objectOffset) - colInfo.ColliderHeight / 2 - PitObject.bounds.size.y / 2);
            }
            else
            {
                Vector3 TEMP = transform.position + new Vector3(colInfo.ColliderWidth / 2 * X_Flip + colInfo.ColliderOffsetX * X_Flip - colInfo.ColliderIndent, colInfo.ColliderOffsetY * Y_Flip, 0);
                Vector3 TEMP2 = transform.position + new Vector3(colInfo.ColliderWidth * 1.5f * X_Flip + colInfo.ColliderOffsetX * X_Flip + colInfo.ColliderIndent, colInfo.ColliderOffsetY * Y_Flip, 0);

                colInfo.DistanceToPit = 1;
                float DistanceToPit2 = 1;

                RaycastHit2D hit = Physics2D.Raycast(TEMP, -Vector2.up * Y_Flip, 0.5f, whatIsGround);
                RaycastHit2D hit2 = Physics2D.Raycast(TEMP2, -Vector2.up * Y_Flip, 0.5f, whatIsGround);

                if (hit.collider != null)
                {
                    colInfo.DistanceToPit = (Mathf.Abs(hit.point.y - transform.position.y - colInfo.ColliderOffsetY * Y_Flip) - colInfo.ColliderHeight / 2);
                }
                if (hit2.collider != null)
                {
                    DistanceToPit2 = (Mathf.Abs(hit2.point.y - transform.position.y - colInfo.ColliderOffsetY * Y_Flip) - colInfo.ColliderHeight / 2);
                }

                if (Mathf.Abs(colInfo.DistanceToPit) > Mathf.Abs(DistanceToPit2))
                    colInfo.DistanceToPit = DistanceToPit2;
            }
        }
        else
        {
            colInfo.DistanceToPit = 1;
        }

        return colInfo.DistanceToPit;
    }
}
