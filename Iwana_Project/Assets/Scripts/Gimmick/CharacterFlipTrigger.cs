using UnityEngine;
using System.Collections;

public class CharacterFlipTrigger : MonoBehaviour
{
    [Tooltip("このオブジェクトが再度トリガーされるまでの遅延時間（秒）")]
    [SerializeField]
    private float reuseDelay = 0.5f;

    [Tooltip("キャラクターが上向きでない場合、上向きに反転させる")]
    [SerializeField]
    private bool uprightFlip = false;

    [Tooltip("キャラクターが逆さでない場合、逆さに反転させる")]
    [SerializeField]
    private bool upsideDownFlip = false;

    [Tooltip("垂直速度をゼロにして即座に反転させる")]
    [SerializeField]
    private bool instaFlip = false;

    [Tooltip("トリガー時に二段ジャンプの回数をリセットする")]
    [SerializeField]
    private bool doubleJumpRefresh = false;

    [Tooltip("オブジェクトが多重にトリガーされないようにする内部制御用フラグ")]
    private bool ableToFlip = true;

    public float ReuseDelay { get { return reuseDelay; } }

    public bool UprightFlip { get { return uprightFlip; } }
    public bool UpsideDownFlip { get { return upsideDownFlip; } }
    public bool InstaFlip { get { return instaFlip; } }
    public bool DoubleJumpRefresh { get { return doubleJumpRefresh; } }
    public bool AbleToFlip { get { return ableToFlip; } private set { ableToFlip = value; } }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == ("Player") && AbleToFlip && other.GetComponent<PlayerCharacter2DBase>() != null)
        {
            StartCoroutine(C_Flip(other.GetComponent<PlayerCharacter2DBase>()));
        }
    }

    IEnumerator C_Flip(PlayerCharacter2DBase GuysScript)
    {
        AbleToFlip = false;

        if (InstaFlip)
            GuysScript.ZeroFallSpeed();

        if (UpsideDownFlip == UprightFlip)
        {
            GuysScript.FlipCharacter();
            if (DoubleJumpRefresh)
                GuysScript.RefreshMultiJump();
        }
        else
        {
            if (UprightFlip)
                if (GuysScript.FlipCharacterUpright() && DoubleJumpRefresh)
                    GuysScript.RefreshMultiJump();

            if (UpsideDownFlip)
                if (GuysScript.FlipCharacterUpsideDown() && DoubleJumpRefresh)
                    GuysScript.RefreshMultiJump();
        }

        yield return new WaitForSeconds(ReuseDelay);
        AbleToFlip = true;
    }


}
