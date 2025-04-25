using UnityEngine;
using System.Collections;

public class CharacterFlipTrigger : MonoBehaviour
{
    [Tooltip("���̃I�u�W�F�N�g���ēx�g���K�[�����܂ł̒x�����ԁi�b�j")]
    [SerializeField]
    private float reuseDelay = 0.5f;

    [Tooltip("�L�����N�^�[��������łȂ��ꍇ�A������ɔ��]������")]
    [SerializeField]
    private bool uprightFlip = false;

    [Tooltip("�L�����N�^�[���t���łȂ��ꍇ�A�t���ɔ��]������")]
    [SerializeField]
    private bool upsideDownFlip = false;

    [Tooltip("�������x���[���ɂ��đ����ɔ��]������")]
    [SerializeField]
    private bool instaFlip = false;

    [Tooltip("�g���K�[���ɓ�i�W�����v�̉񐔂����Z�b�g����")]
    [SerializeField]
    private bool doubleJumpRefresh = false;

    [Tooltip("�I�u�W�F�N�g�����d�Ƀg���K�[����Ȃ��悤�ɂ����������p�t���O")]
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
