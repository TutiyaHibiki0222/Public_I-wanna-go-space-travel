using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attribute.Add;

[System.Serializable]
public class GravitySettings
{
   [Tooltip("�ȉ��̌��ʂ�L��?")]                   
   public bool ChangeGravity = false;
   [Tooltip("�d�͔{��"), ShowIf("ChangeGravity")]
   public float gravityMultiplier = 1;
}

[System.Serializable]
public class JumpSettings
{
    [Tooltip("�ȉ��̌��ʂ�L��?")] 
    public bool ChangeJumpforce = false;
    [Tooltip("�W�����v�{��"), ShowIf("ChangeJumpforce")]      
    public float jumpforceMultiplier = 1;
    [Tooltip("�ȉ��̌��ʂ�L��?")] 
    public bool ChangeMaxNumberOfExtraJumps = false;
    [Tooltip("�W�����v�񐔏��"),ShowIf("ChangeMaxNumberOfExtraJumps")]  
    public int newMaxNumberOfExtraJumps = 1;
    [Tooltip("�����W�����v�L��?")] 
    public bool infinityJump = false;
}

[System.Serializable]
public class SpeedSettings
{
    [Tooltip("�ȉ��̌��ʂ�L��?")] 
    public bool  ChangeSpeed = false;
    [Tooltip("�ő呬�x(X)"), ShowIf("ChangeSpeed")]       
    public float speedMultiplier = 1;
    [Tooltip("�ȉ��̌��ʂ�L��?")] 
    public bool ChangeMaxFallSpeed = false;
    [Tooltip("�ő呬�x �������x"), ShowIf("ChangeMaxFallSpeed")] 
    public float newMaxFallSpeed = -9.0f;
}

[System.Serializable]
public class MiscSettings
{
    [Tooltip("����shot�L��?")]           public bool autoFire = false;
    [Tooltip("��\���L��?")]             public bool OneTimeActivate = false;
    [Tooltip("�ݒ�����Z�b�g�L��?")]     public bool RevertOnExit = false;
    [Tooltip("�W�����v���Z�b�g�L��?")]   public bool RefreshMultiJumpOnExit = false;
}

public class AreaModifier : MonoBehaviour
{
    [Header("�{��")]
    public GravitySettings gravitySettings;

    [Header("�W�����v�Ɋւ���ݒ�")]
    public JumpSettings jumpSettings;

    [Header("���x�Ɋւ���ݒ�")]
    public SpeedSettings speedSettings;

    [Header("���̑��̐ݒ�")]
    public MiscSettings miscSettings;

    private PlayerCharacter2DBase GuysScript;

    static int numberOfTriggered = 0;

    void Awake()
    {
        numberOfTriggered = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerCharacter2DBase>() != null)
        {
            GuysScript = other.gameObject.GetComponent<PlayerCharacter2DBase>();

            if (gravitySettings.ChangeGravity)
                GuysScript.gameMultiplier.gravityMultiplier = gravitySettings.gravityMultiplier;

            if (jumpSettings.ChangeJumpforce)
                GuysScript.gameMultiplier.jampMultiplier = jumpSettings.jumpforceMultiplier;

            if (jumpSettings.ChangeMaxNumberOfExtraJumps)
                GuysScript.SetMultiJumps(jumpSettings.newMaxNumberOfExtraJumps);

            if (jumpSettings.infinityJump)
                GuysScript.jumpConfig.isInfinityJump = true;

            if (speedSettings.ChangeSpeed)
                GuysScript.gameMultiplier.moveMultiplier = speedSettings.speedMultiplier;

            if (speedSettings.ChangeMaxFallSpeed)
                GuysScript.player.maxFallSpeed = speedSettings.newMaxFallSpeed;

            // Miscellaneous Settings
            if (miscSettings.autoFire && GuysScript.GetComponent<BulletSpawner>() != null)
                GuysScript.GetComponent<BulletSpawner>().AutoFire = true;

            if (miscSettings.OneTimeActivate)
                gameObject.SetActive(false);

            numberOfTriggered++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerCharacter2DBase>() != null && miscSettings.RevertOnExit)
        {
            if (numberOfTriggered == 1)
            {
                Reset(other.GetComponent<PlayerCharacter2DBase>());
            }

            numberOfTriggered--;
        }
    }

    void Reset(PlayerCharacter2DBase aPlayer)
    {
        // Gravity Settings
        if (gravitySettings.ChangeGravity)
            aPlayer.ResetGravityMultiplier();

        // Jump Settings
        if (jumpSettings.ChangeJumpforce)
            aPlayer.ResetJumpforceMultipler();

        if (jumpSettings.ChangeMaxNumberOfExtraJumps)
            aPlayer.ResetMultiJump();

        if (jumpSettings.infinityJump)
            aPlayer.jumpConfig.isInfinityJump = false;

        // Speed Settings
        if (speedSettings.ChangeSpeed)
            aPlayer.ResetMovementMultiplier();

        if (speedSettings.ChangeMaxFallSpeed)
            aPlayer.ResetMaxFallSpeed();

        // Miscellaneous Settings
        if (miscSettings.autoFire && aPlayer.GetComponent<BulletSpawner>() != null)
            aPlayer.GetComponent<BulletSpawner>().AutoFire = false;

        if (miscSettings.RefreshMultiJumpOnExit)
            aPlayer.RefreshMultiJump();

        if (miscSettings.OneTimeActivate)
            gameObject.SetActive(true);

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    void Reset()
    {
        numberOfTriggered = 0;
        if (GuysScript != null)
            Reset(GuysScript);
    }
}
