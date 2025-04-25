using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attribute.Add;

[System.Serializable]
public class GravitySettings
{
   [Tooltip("以下の効果を有効?")]                   
   public bool ChangeGravity = false;
   [Tooltip("重力倍率"), ShowIf("ChangeGravity")]
   public float gravityMultiplier = 1;
}

[System.Serializable]
public class JumpSettings
{
    [Tooltip("以下の効果を有効?")] 
    public bool ChangeJumpforce = false;
    [Tooltip("ジャンプ倍率"), ShowIf("ChangeJumpforce")]      
    public float jumpforceMultiplier = 1;
    [Tooltip("以下の効果を有効?")] 
    public bool ChangeMaxNumberOfExtraJumps = false;
    [Tooltip("ジャンプ回数上限"),ShowIf("ChangeMaxNumberOfExtraJumps")]  
    public int newMaxNumberOfExtraJumps = 1;
    [Tooltip("無限ジャンプ有効?")] 
    public bool infinityJump = false;
}

[System.Serializable]
public class SpeedSettings
{
    [Tooltip("以下の効果を有効?")] 
    public bool  ChangeSpeed = false;
    [Tooltip("最大速度(X)"), ShowIf("ChangeSpeed")]       
    public float speedMultiplier = 1;
    [Tooltip("以下の効果を有効?")] 
    public bool ChangeMaxFallSpeed = false;
    [Tooltip("最大速度 落下速度"), ShowIf("ChangeMaxFallSpeed")] 
    public float newMaxFallSpeed = -9.0f;
}

[System.Serializable]
public class MiscSettings
{
    [Tooltip("自動shot有効?")]           public bool autoFire = false;
    [Tooltip("非表示有効?")]             public bool OneTimeActivate = false;
    [Tooltip("設定をリセット有効?")]     public bool RevertOnExit = false;
    [Tooltip("ジャンプリセット有効?")]   public bool RefreshMultiJumpOnExit = false;
}

public class AreaModifier : MonoBehaviour
{
    [Header("倍率")]
    public GravitySettings gravitySettings;

    [Header("ジャンプに関する設定")]
    public JumpSettings jumpSettings;

    [Header("速度に関する設定")]
    public SpeedSettings speedSettings;

    [Header("その他の設定")]
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
