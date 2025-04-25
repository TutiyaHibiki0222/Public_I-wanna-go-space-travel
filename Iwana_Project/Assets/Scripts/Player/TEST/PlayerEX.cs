using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEX : PlayerCharacter2DBase
{
    private float Physics_ColliderBuffer = 0.011f;
    private float MinPenetrationForPenalty = 0.004f;
    new void Awake()
    {
        base.Awake();

        ColliderBuffer = Physics_ColliderBuffer;
        Physics2D.defaultContactOffset = MinPenetrationForPenalty;
        TheRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public override void Move(float MoveDirection)
    {
        if (!DisablePlayerMove)
        {
            CheckWall();
            if      (MoveDirection > 0 && !FacingRight)
                Flip();
            else if (MoveDirection < 0 &&  FacingRight)
                Flip();
            else
            {
                X_Speed = MoveDirection * MoveSpeed * MovementMultiplier * Time.deltaTime;
                MoveVelocity(MoveDirection);
            }
        }
        else
            MoveVelocity(0);

    }
    void MoveVelocity(float MoveDirection)
    {
        if (MoveDirection != 0)
        {
            TheRigidbody2D.velocity = new Vector2(MoveDirection * MoveSpeed * MovementMultiplier, TheRigidbody2D.velocity.y);
        }
        else
        {
            TheRigidbody2D.velocity = new Vector2(0, TheRigidbody2D.velocity.y);
        }
    }

    public override void Jump()
    {
        if (!DisablePlayerJump)
        {
            CheckGround();
            if ((OnGround || CanMultiJump() || InfinityJump))
            {
                if (OnGround)
                {
                    RefreshMultiJump();

                    Y_Speed = JumpSpeed * JumpforceMultiplier;
                    TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, JumpSpeed * JumpforceMultiplier);

                    if (MusicController.instance != null)
                        MusicController.instance.OneShotAudio(sound.jump1Sound);
                }
                else
                {
                    Y_Speed = JumpSpeed2 * JumpforceMultiplier;
                    TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, JumpSpeed2 * JumpforceMultiplier);

                    if (MusicController.instance != null)
                        MusicController.instance.OneShotAudio(sound.jump2Sound);

                    MultiJumpUsed();
                }
            }
            OnGround = false;
        }
    }
    /// <summary>
    /// èdóÕÇÃåvéZ.
    /// </summary>
    public override void ApplyGravity()
    {
        float maxFallSpeed_gravity = MaxFallSpeed * GravityMultiplier;
        float NewFallSpeed         = Y_Speed - Gravity * GravityMultiplier;

        CheckGround();                                                   
        CheckCeiling();
        if (NewFallSpeed < maxFallSpeed_gravity && IsUpright)
        {
            NewFallSpeed = maxFallSpeed_gravity;
        }
        if (NewFallSpeed > maxFallSpeed_gravity && !IsUpright)
        {
            NewFallSpeed = maxFallSpeed_gravity;
        }
        float FlipMultiplier = 1;
        if (!IsUpright)
            Å@FlipMultiplier = -1;

        if (NewFallSpeed * FlipMultiplier > 0)         
        {
            float HowFarTillStop_Ceiling = DistanceToCeiling - ColliderBuffer;
            float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;

            if (Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ceiling && AllowPreCheckCeiling)
            {
                TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, 0);
                gameObject.transform.position += new Vector3(0, HowFarTillStop_Ceiling * FlipMultiplier, 0);
                Y_Speed = 0;
                HitCeiling = true;
            }
            else
            {
                Y_Speed = NewFallSpeed;
                TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, NewFallSpeed);
            }

            if (HitCeiling)
            {
                Y_Speed = 0;
                TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, 0);
                
            }
        }
        else 
        {
            float HowFarTillStop_Ground = DistanceToGround - ColliderBuffer;
            float NewFallSpeed_Pixels = NewFallSpeed * Time.deltaTime;

            if (HowFarTillStop_Ground > 0.005f)
            {
                if (Mathf.Abs(2 * NewFallSpeed_Pixels) > HowFarTillStop_Ground)
                {
                    if (Mathf.Abs(NewFallSpeed_Pixels) > HowFarTillStop_Ground)
                    {
                        TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, 0);
                        gameObject.transform.position -= new Vector3(0, HowFarTillStop_Ground * FlipMultiplier, 0);
                        OnGround = true;
                        Y_Speed = 0;
                    }
                    else
                    {
                        TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, 0);
                        gameObject.transform.position += new Vector3(0, NewFallSpeed * Time.deltaTime, 0);
                        Y_Speed = NewFallSpeed;
                    }
                }
                else
                {
                    Y_Speed = NewFallSpeed;
                    TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, NewFallSpeed);
                }
            }
            else
            {
                OnGround = true;
                Y_Speed = 0;
                TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, 0);
            }  
        }
    }
    public override void ShortJumping()
    {
        Y_Speed = Y_Speed * 0.45f;
        TheRigidbody2D.velocity = new Vector2(TheRigidbody2D.velocity.x, Y_Speed);
    }
    public override void SetMinPenetration()
    {
        Physics2D.defaultContactOffset = MinPenetrationForPenalty;
    }
}
