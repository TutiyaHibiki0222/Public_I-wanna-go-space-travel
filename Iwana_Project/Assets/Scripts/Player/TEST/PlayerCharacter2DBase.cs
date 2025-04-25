using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum InputEvent
{
    DOWN, UP
}

[Serializable, Tooltip("�L�[���͐ݒ�class")]
class InputKeyCode 
{
    [SerializeField, Tooltip("��")]       public KeyCode left  = KeyCode.LeftArrow;
    [SerializeField, Tooltip("�E")]       public KeyCode right = KeyCode.RightArrow;
    [SerializeField, Tooltip("�W�����v")] public KeyCode jump  = KeyCode.LeftShift;

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
[Serializable, Tooltip("�T�E���h�ݒ�class")]
public class SoundSE
{
    [SerializeField, Tooltip("��i�W�����vSE")] public AudioClip jump1Sound;
    [SerializeField, Tooltip("��i�W�����vSE")] public AudioClip jump2Sound;
}
[Serializable, Tooltip("�T�E���h�ݒ�class")]
public class GameMultiplier
{
    [SerializeField, Tooltip("�d�͔{��")]     public float gravityMultiplier   = 1;
    [SerializeField, Tooltip("�ړ��{��")]     public float moveMultiplier      = 1;
    [SerializeField, Tooltip("�W�����v�{��")] public float jampMultiplier      = 1;
}
[Serializable, Tooltip("�v���C���[�̐ݒ�class")]
public class PlayerConfig
{
    [SerializeField, Tooltip("��i�W�����v")]        public float jumpSpeed   =  8.5f;
    [SerializeField, Tooltip("��i�W�����v")]        public float jumpSpeed2  =  7.0f;
    [SerializeField, Tooltip("�ړ����x")]            public float moveSpeed   =  3.0f;
    [SerializeField, Tooltip("�ő嗎�����x")]        public float maxFallSpeed= -9.0f;
    [SerializeField, Tooltip("�d��")]                public float gravity     =  0.4f;

    public void Scale(float multiplier)
    {
        jumpSpeed       *= multiplier;
        jumpSpeed2      *= multiplier;
        moveSpeed       *= multiplier;
        maxFallSpeed    *= multiplier;
        gravity         *= multiplier;
    }

}
[Serializable, Tooltip("�W�����v�ݒ�class")]
public class JampConfig
{
    [SerializeField, Tooltip("�����W�����v")]       public bool isInfinityJump   = false;
    [HideInInspector, Tooltip("�W�����vCount")]     public int jumpCount         = 0;
    [SerializeField, Tooltip("�ǉ��W�����v�ő吔")] public int  maxJanpCount     = 1;

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

[Serializable, Tooltip("�Q�[���ݒ�class")]
class GM 
{
    [SerializeField,  Tooltip("fps�͂ǂꂮ�炢?")]       private float FramesPerSecond  = 50;  
    [SerializeField,  Tooltip("�P�ʂ�����̃s�N�Z����")] private float PixelsPerUnit    = 64;
    [Tooltip("get FramesPerSecond / PixelsPerUnit")] public float GMtoUnityConversion { get { return FramesPerSecond / PixelsPerUnit; } }
}

public abstract class PlayerCharacter2DBase : MonoBehaviour
{
    [SerializeField, Tooltip("�ǂ̃��C���[���Ƃ��Ĕ���")] LayerMask whatIsGround;
    [SerializeField, Tooltip("�L�[�ݒ�")]       InputKeyCode input;
    [SerializeField, Tooltip("�W�����vSE")]     public SoundSE sound;
    [SerializeField, Tooltip("�{���␳")]       public GameMultiplier gameMultiplier;
    [SerializeField, Tooltip("�v���C���[�ݒ�")] public PlayerConfig player;

    [Tooltip("�ړ�����")] private float moveDirection = 0;
    [Tooltip("�ړ��֐��Ăׂ邩")] private bool isMoving = false;
    [Tooltip("�W�����v�֐��Ăׂ邩")] private bool isJumping = false;

    [Tooltip("���̑��x")] private float xSpeed = 0;
    [Tooltip("�c�̑��x")] private float ySpeed = 0;

    [Tooltip("�o�b�N�A�b�v�d�͏搔")] private float B_gravityMultiplier = 1;
    [Tooltip("�ő嗎�����x�̃o�b�N�A�b�v")] private float B_maxFallSpeed = 1;

    [SerializeField, Tooltip("�W�����v�ݒ�")] public JampConfig jumpConfig;
    [SerializeField, Tooltip("�Q�[���ݒ�Ȃǂ̏ڍ�")] GM gm;
    /// <summary>
    ///  �������Z�̑���Ƀs�N�Z���̓������g�p���ׂ���
    /// </summary>
    public bool IsPixelMove { get; set; }
    /// <summary>
    /// �w�������͂𖳌��ɂł��܂�
    /// </summary>
    public bool DisablePlayerMove { get; set; }
    /// <summary>
    /// �W�����v�𖳌��ɂł��܂�
    /// </summary>
    public bool DisablePlayerJump { get; set; }
    /// <summary>
    /// ���˔\�͂𖳌��ɂ��邱�Ƃ��ł��܂�
    /// </summary>
    public bool DisablePlayerShot { get; set; }

    [Tooltip("CollisionCalculation")]   private CollisionCalculation calculate;
    [Tooltip("�A�j���[�V����")]         private Animator anim;
    [Tooltip("�������Z")]               private Rigidbody2D theRigidbody2D;

    [Tooltip("���݃v���C���[���E�������Ă��邩�ǂ����𔻒肵�܂�")]
    private bool facingRight = true;

    [Tooltip("���݃v���C���[�������������i�㉺�j�������Ă��邩�ǂ����𔻒肵�܂�")]
    private bool isUpright = true;

    [Tooltip("���̂ɋ߂��Ƃ݂Ȃ������i臒l�j���w�肵�܂�")]
    private float closeThreshold = 0.02f;

    /// <summary>
    /// �G�f�B�^�ݒ�Œ������ꂽ�A�R���C�_�[�ɑ΂���ǉ��̃o�b�t�@����
    /// </summary>
    public float ColliderBuffer { get; set; }

    [Tooltip("�n�ʂ܂ł̌v�Z���ꂽ����")]
    private float distanceToGround = 1;

    [Tooltip("�ǂ܂ł̌v�Z���ꂽ����")]
    private float distanceToWall = 1;

    [Tooltip("�V��܂ł̌v�Z���ꂽ����")]
    private float distanceToCeiling = 1;

    [Tooltip("�L�����N�^�[���n�ʂɐڒn���Ă��邩�ǂ������肵�܂�")]
    private bool onGround = false;

    [Tooltip("�L�����N�^�[���V��ɐڐG�������ǂ������肵�܂�")]
    private bool hitCeiling = false;

    [Tooltip("�L�����N�^�[���ǂɐڐG�������ǂ������肵�܂�")]
    private bool hitWall = false;

    [Tooltip("�ړ��ɑ΂��ĕǂ̎��O�`�F�b�N��L���ɂ��邩�ǂ�����ݒ肵�܂�")]
    private bool allowPreCheckWall = true;

    [Tooltip("�ړ��ɑ΂��Ēn�ʂ̎��O�`�F�b�N��L���ɂ��邩�ǂ�����ݒ肵�܂�")]
    private bool allowPreCheckGround = true;

    [Tooltip("�ړ��ɑ΂��ēV��̎��O�`�F�b�N��L���ɂ��邩�ǂ�����ݒ肵�܂�")]
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
        // ���g��Rigidbody2D���擾��������.
        theRigidbody2D = GetComponent<Rigidbody2D>();
        theRigidbody2D.velocity = Vector2.zero;
        // Player�̐ݒ�� *= �� Gm�� �{�����~.
        player.Scale(gm.GMtoUnityConversion);
        // �o�b�N�A�b�v�p.
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
    // �p���K�{�֐�.
    public abstract void Move(float moveDirection);
    public abstract void Jump();
    public abstract void ApplyGravity();
    public abstract void ShortJumping();
    public abstract void SetMinPenetration();


    /****************************************************************************************************************/
    /// <summary>
    /// �A�j���[�V��������.
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
    ///�n��ɐG��Ă��邩.
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
    /// �ǂɐG��Ă��邩�B
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
    /// �V��ɐG��Ă��邩�B
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
