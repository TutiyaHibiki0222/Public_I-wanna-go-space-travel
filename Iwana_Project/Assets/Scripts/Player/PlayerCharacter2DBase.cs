using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum InputEvent
{
    DOWN, UP
}

[Serializable, Tooltip("キー入力設定class")]
class InputKeyCode 
{
    [SerializeField, Tooltip("左")]       public KeyCode left  = KeyCode.LeftArrow;
    [SerializeField, Tooltip("右")]       public KeyCode right = KeyCode.RightArrow;
    [SerializeField, Tooltip("ジャンプ")] public KeyCode jump  = KeyCode.LeftShift;

    public float MoveEvent()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if      (Input.GetKey(left))
            move = -1;
        else if (Input.GetKey(right))
            move =  1;
        return move;
    }

    public bool JumpEvent(InputEvent e)
    {
        switch (e)
        {
            case InputEvent.DOWN:
                return Input.GetKeyDown(jump);
            case InputEvent.UP: 
                return Input.GetKeyUp(jump);
            default: return false;
        }
    }
}
[Serializable, Tooltip("サウンド設定class")]
public class SoundSE
{
    [SerializeField, Tooltip("一段ジャンプSE")] public AudioClip jump1Sound;
    [SerializeField, Tooltip("二段ジャンプSE")] public AudioClip jump2Sound;
}
[Serializable, Tooltip("サウンド設定class")]
public class GameMultiplier
{
    [SerializeField, Tooltip("重力倍率")]     public float gravityMultiplier   = 1;
    [SerializeField, Tooltip("移動倍率")]     public float moveMultiplier      = 1;
    [SerializeField, Tooltip("ジャンプ倍率")] public float jampMultiplier      = 1;
}
[Serializable, Tooltip("プレイヤーの設定class")]
public class PlayerConfig
{
    [SerializeField, Tooltip("一段ジャンプ")]        public float jumpSpeed   =  8.5f;
    [SerializeField, Tooltip("二段ジャンプ")]        public float jumpSpeed2  =  7.0f;
    [SerializeField, Tooltip("移動速度")]            public float moveSpeed   =  3.0f;
    [SerializeField, Tooltip("最大落下速度")]        public float maxFallSpeed= -9.0f;
    [SerializeField, Tooltip("重力")]                public float gravity     =  0.4f;

    public void Scale(float multiplier)
    {
        jumpSpeed       *= multiplier;
        jumpSpeed2      *= multiplier;
        moveSpeed       *= multiplier;
        maxFallSpeed    *= multiplier;
        gravity         *= multiplier;
    }

}
[Serializable, Tooltip("ジャンプ設定class")]
public class JampConfig
{
    [SerializeField, Tooltip("無限ジャンプ")]       public bool isInfinityJump   = false;
    [HideInInspector, Tooltip("ジャンプCount")]     public int jumpCount         = 0;
    [SerializeField, Tooltip("追加ジャンプ最大数")] public int  maxJanpCount     = 1;

    public bool CanMultiJump()
    {
        return jumpCount > 0;
    }

    public void RefreshMultiJumpa()
    {
        if (jumpCount <= maxJanpCount)
            jumpCount = maxJanpCount;
    }
}

[Serializable, Tooltip("ゲーム設定class")]
class GM 
{
    [SerializeField,  Tooltip("fpsはどれぐらい?")]       private float FramesPerSecond  = 50;  
    [SerializeField,  Tooltip("単位あたりのピクセル数")] private float PixelsPerUnit    = 64;
    [Tooltip("get FramesPerSecond / PixelsPerUnit")] public float GMtoUnityConversion { get { return FramesPerSecond / PixelsPerUnit; } }
}

public abstract class PlayerCharacter2DBase : MonoBehaviour
{
    [SerializeField, Tooltip("どのレイヤー床として判定")] LayerMask whatIsGround;
    [SerializeField, Tooltip("キー設定")]       InputKeyCode input;
    [SerializeField, Tooltip("ジャンプSE")]     public SoundSE sound;
    [SerializeField, Tooltip("倍率補正")]       public GameMultiplier gameMultiplier;
    [SerializeField, Tooltip("プレイヤー設定")] public PlayerConfig player;

    [Tooltip("移動方向")] private float moveDirection = 0;
    [Tooltip("移動関数呼べるか")] private bool isMoving = false;
    [Tooltip("ジャンプ関数呼べるか")] private bool isJumping = false;

    [Tooltip("横の速度")] private float xSpeed = 0;
    [Tooltip("縦の速度")] private float ySpeed = 0;

    [Tooltip("バックアップ重力乗数")] private float B_gravityMultiplier = 1;
    [Tooltip("最大落下速度のバックアップ")] private float B_maxFallSpeed = 1;

    [SerializeField, Tooltip("ジャンプ設定")] public JampConfig jumpConfig;
    [SerializeField, Tooltip("ゲーム設定などの詳細")] GM gm;
    /// <summary>
    ///  物理演算の代わりにピクセルの動きを使用すべきか
    /// </summary>
    public bool IsPixelMove { get; set; }
    /// <summary>
    /// 指向性入力を無効にできます
    /// </summary>
    public bool DisablePlayerMove { get; set; }
    /// <summary>
    /// ジャンプを無効にできます
    /// </summary>
    public bool DisablePlayerJump { get; set; }
    /// <summary>
    /// 発射能力を無効にすることができます
    /// </summary>
    public bool DisablePlayerShot { get; set; }

    [Tooltip("CollisionCalculation")]   private CollisionCalculation calculate;
    [Tooltip("アニメーション")]         private Animator anim;
    [Tooltip("物理演算")]               private Rigidbody2D theRigidbody2D;

    [Tooltip("現在プレイヤーが右を向いているかどうかを判定します")]
    private bool facingRight = true;

    [Tooltip("現在プレイヤーが正しい向き（上下）を向いているかどうかを判定します")]
    private bool isUpright = true;

    [Tooltip("物体に近いとみなす距離（閾値）を指定します")]
    private float closeThreshold = 0.02f;

    /// <summary>
    /// エディタ設定で調整された、コライダーに対する追加のバッファ距離
    /// </summary>
    public float ColliderBuffer { get; set; }

    [Tooltip("地面までの計算された距離")]
    private float distanceToGround = 1;

    [Tooltip("壁までの計算された距離")]
    private float distanceToWall = 1;

    [Tooltip("天井までの計算された距離")]
    private float distanceToCeiling = 1;

    [Tooltip("キャラクターが地面に接地しているかどうか判定します")]
    private bool onGround = false;

    [Tooltip("キャラクターが天井に接触したかどうか判定します")]
    private bool hitCeiling = false;

    [Tooltip("キャラクターが壁に接触したかどうか判定します")]
    private bool hitWall = false;

    [Tooltip("移動に対して壁の事前チェックを有効にするかどうかを設定します")]
    private bool allowPreCheckWall = true;

    [Tooltip("移動に対して地面の事前チェックを有効にするかどうかを設定します")]
    private bool allowPreCheckGround = true;

    [Tooltip("移動に対して天井の事前チェックを有効にするかどうかを設定します")]
    private bool allowPreCheckCeiling = true;

    /****************************************************************************************************************/

    public float X_Speed { get { return xSpeed; } set { xSpeed = value; } }
    public float Y_Speed { get { return ySpeed; } set { ySpeed = value; } }

    public float GravityMultiplier { get { return gameMultiplier.gravityMultiplier; } set { gameMultiplier.gravityMultiplier = value; } }
    public float MovementMultiplier { get { return gameMultiplier.moveMultiplier; } set { gameMultiplier.moveMultiplier = value; } }
    public float JumpforceMultiplier { get { return gameMultiplier.jampMultiplier; } set { gameMultiplier.jampMultiplier = value; } }

    public float JumpSpeed { get { return player.jumpSpeed; } }
    public float JumpSpeed2 { get { return player.jumpSpeed2; } }
    public float MoveSpeed { get { return player.moveSpeed; } }
    public float MaxFallSpeed { get { return player.maxFallSpeed; } set { player.maxFallSpeed = value * gm.GMtoUnityConversion; } }
    public float Gravity { get { return player.gravity; } }

    public bool InfinityJump { get { return jumpConfig.isInfinityJump; } set { jumpConfig.isInfinityJump = value; } }

    public bool IsMoving { get { return isMoving; } }
    public bool IsJumping { get { return isJumping; } }

    public Rigidbody2D TheRigidbody2D { get { return theRigidbody2D; } }

    public bool FacingRight { get { return facingRight; } }
    public bool IsUpright { get { return isUpright; } }

    public float DistanceToGround { get { return distanceToGround; } }
    public float DistanceToWall { get { return distanceToWall; } }
    public float DistanceToCeiling { get { return distanceToCeiling; } }

    public bool OnGround { get { return onGround; } set { onGround = value; } }
    public bool HitCeiling { get { return hitCeiling; } set { hitCeiling = value; } }
    public bool HitWall { get { return hitWall; } set { hitWall = value; } }

    public bool AllowPreCheckGround { get { return allowPreCheckGround; } set { allowPreCheckGround = value;    } }
    public bool AllowPreCheckWall   { get { return allowPreCheckWall;   } set { allowPreCheckWall = value;      } }
    public bool AllowPreCheckCeiling{ get { return allowPreCheckCeiling;} set { allowPreCheckCeiling = value;   } }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////  
    public void Awake()
    {
        // 自身のRigidbody2Dを取得し初期化.
        theRigidbody2D = GetComponent<Rigidbody2D>();
        theRigidbody2D.velocity = Vector2.zero;
        // Playerの設定を *= し Gmの 倍率を×.
        player.Scale(gm.GMtoUnityConversion);
        // バックアップ用.
        B_maxFallSpeed = player.maxFallSpeed;

        IsPixelMove = false;
        DisablePlayerMove = false;
        DisablePlayerJump = false;
        DisablePlayerShot = false;

        anim = GetComponent<Animator>();

        calculate = GetComponent<CollisionCalculation>();
        if (!calculate) calculate = gameObject.AddComponent<CollisionCalculation>();
        calculate.ChangeLayerMask(whatIsGround);
        calculate.Init();
    }

    void OnDestroy()
    {

    }

    void Reset()
    {
        FlipCharacterUpright();
        RefreshMultiJump();
        isMoving = false;
        isJumping = false;
        ySpeed = 0;
        xSpeed = 0;

        theRigidbody2D.velocity = Vector3.zero;

        IsPixelMove = false;
        gameObject.transform.parent = null;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Move(moveDirection);
            isMoving = false;
        }

        if (isJumping)
        {
            Jump();
            isJumping = false;
        }

        ApplyGravity();
        SetAnimationVariables();

        if (X_Speed == 0 && theRigidbody2D.velocity.x != 0)
        {
            theRigidbody2D.velocity = new Vector2(0, theRigidbody2D.velocity.y);
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            moveDirection = input.MoveEvent();
            if (X_Speed != moveDirection || moveDirection != 0)
            {
                isMoving = true;
            }
        }

        if (input.JumpEvent(InputEvent.DOWN) && !isJumping)
        {
            isJumping = true;
        }

        if (input.JumpEvent(InputEvent.UP) && !isJumping)
        {
            ShortJumping();
        }
    }
    // 継承必須関数.
    public abstract void Move(float moveDirection);
    public abstract void Jump();
    public abstract void ApplyGravity();
    public abstract void ShortJumping();
    public abstract void SetMinPenetration();


    /****************************************************************************************************************/
    /// <summary>
    /// アニメーション処理.
    /// </summary>
    public void SetAnimationVariables()
    {
        anim.SetBool("Ground",onGround);
        anim.SetFloat("horizontalSpeed", Mathf.Abs(moveDirection));

        if (isUpright)
            anim.SetFloat("fallSpeed",  Y_Speed / gm.GMtoUnityConversion);
        else
            anim.SetFloat("fallSpeed", -Y_Speed / gm.GMtoUnityConversion);
    }
    /****************************************************************************************************************/
    /// <summary>
    ///地上に触れているか.
    /// </summary>
    public void CheckGround()
    {
        distanceToGround = calculate.GroundCheck(IsUpright);
        if ((distanceToGround < (closeThreshold + ColliderBuffer)) && (ySpeed * GravityMultiplier <= 0.0))
        {
            onGround = true;
            RefreshMultiJump();
        }
        else
            onGround = false;
    }
    /// <summary>
    /// 壁に触れているか。
    /// </summary>
    public void CheckWall()
    {
        distanceToWall = calculate.WallCheck(isUpright, facingRight);
        if (distanceToWall < (closeThreshold + ColliderBuffer))
            hitWall = true;            
        else
            hitWall = false;
    }
    /// <summary>
    /// 天井に触れているか。
    /// </summary>
    public void CheckCeiling()
    {
        distanceToCeiling = calculate.CeilingCheck(isUpright);
        if (distanceToCeiling < (closeThreshold + ColliderBuffer))
            hitCeiling = true;
        else
            hitCeiling = false;

        if (distanceToCeiling < 0)
        {
            distanceToCeiling = 0;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Flip()
    {
        facingRight = !facingRight;                 

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool FlipCharacterUpright()
    {
        if (!isUpright) 
        {
            isUpright = !isUpright;

            theRigidbody2D.gravityScale *= -1;
            GravityMultiplier   = 1;
            JumpforceMultiplier = 1;

            Vector3 theScale = transform.localScale;
            theScale.y = 1;
            transform.localScale = theScale;

            return true;
        }
        else
            return false;
    }

    public bool FlipCharacterUpsideDown()
    {
        if (isUpright)             
        {
            isUpright = !isUpright;

            theRigidbody2D.gravityScale *= -1;
            GravityMultiplier   *= -1;
            JumpforceMultiplier *= -1;

            Vector3 theScale = transform.localScale;
            theScale.y *= -1;
            transform.localScale = theScale;

            return true;
        }
        else
            return false;
    }

    public void FlipCharacter()
    {
        isUpright = !isUpright;

        theRigidbody2D.gravityScale *= -1;
        GravityMultiplier   *= -1;
        JumpforceMultiplier *= -1;

        Vector3 theScale = transform.localScale;
        theScale.y *= -1;
        transform.localScale = theScale;
    }

    public bool CanMultiJump()    
    {
        return jumpConfig.CanMultiJump();
    }

    public void MultiJumpUsed() 
    {
        jumpConfig.jumpCount--;
    }

    public void RefreshMultiJump()
    {
        jumpConfig.RefreshMultiJumpa();
    }

    public void AddToMultiJump(int NumberOfJumps, bool GoOverCap)
    {
        jumpConfig.jumpCount += NumberOfJumps;
        if (jumpConfig.jumpCount > jumpConfig.maxJanpCount && !GoOverCap)
            jumpConfig.jumpCount = jumpConfig.maxJanpCount;

    }

    public void SetMultiJumps(int NumberOfExtraJumps)
    {
       jumpConfig.maxJanpCount = NumberOfExtraJumps;
    }
    public void ZeroFallSpeed()
    {
        ySpeed = 0;
        theRigidbody2D.velocity = new Vector2(theRigidbody2D.velocity.x, 0);
    }
    public void StopHorizontalMovement()
    {
        xSpeed = 0;
        theRigidbody2D.velocity = new Vector2(0, theRigidbody2D.velocity.y);
    }

    public void ResetMultiJump() 
    {
        jumpConfig.maxJanpCount = 1;
    }
    public void SaveGravityMultiplier()
    {
        B_gravityMultiplier = gameMultiplier.gravityMultiplier;
    }
    public void Load_GravityMultiplier()
    {
        gameMultiplier.gravityMultiplier = B_gravityMultiplier;
    }

    public void ResetGravityMultiplier()
    {
        gameMultiplier.gravityMultiplier = 1;
    }
    public void ResetMaxFallSpeed()
    {
        player.maxFallSpeed = B_maxFallSpeed;
    }
    public void ResetJumpforceMultipler()
    {
        gameMultiplier.jampMultiplier = 1;
    }
    public void ResetMovementMultiplier()
    {
        gameMultiplier.moveMultiplier = 1;
    }

}
