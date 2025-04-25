using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    /************�ϐ��錾*************/
    [SerializeField] private float px = 0.0f;               // �ʏ�ړ�.
    [SerializeField] private float psy = 0.0f;              // ���W�����v.
    [SerializeField] private float pby = 0.0f;              // ��W�����v.
    [SerializeField] private float j_HoldTime = 0.0f;       // �W�����v�{�^������������������.
    [SerializeField] private float j_HoldDura = 0.0f;       // �ő�W�����v�p������.
    [SerializeField] private int maxJump = 2;               // ��i�W�����v.
    [SerializeField] private bool isGround;                 // �ڒn����.
    [SerializeField] private bool isJump;                   // �W�����v����.
    [SerializeField] private bool isDeth;                   // ���S����.
    [SerializeField] GameObject bullet;                     // �e�̃Z�b�g.
    //[SerializeField] GameObject dethPrefab;                 // �����Ԃ��̃v���t�@�u.
    [SerializeField] private Vector3 initialPosition;       // �����ʒu�ۑ�.
    [SerializeField] private AudioSource jump;              // �W�����vSE.
    [SerializeField] private AudioSource deth;              // ���SSE.
    [SerializeField] private AudioSource shot;              // �V���b�gSE.
    [SerializeField] private SpriteRenderer spriteRenderer; // �v���C���[�̃X�v���C�g
    [SerializeField] private new Collider2D collider2D;     // �v���C���[�̃R���C�_�[
    [SerializeField] private Transform respownPoint;        // ���X�|�[���n�_.

    private Animator anim;               // �A�j���[�V����.
    private Rigidbody2D rb;              // �����蔻��.
    private int JumpCount = 0;           // ���݂̃W�����v��.

    // ���@�̃��[�h.
    enum MODE{ NONE, ATTACK, RESET, DETH, STOP };
    private MODE mode = MODE.NONE;

    /***************�֐�****************/
    // Start is called before the first frame update
    void Start()
    {
        mode = MODE.NONE;                     // �������[�h.
        initialPosition = transform.position; // �v���C���[�̏����ʒu��ۑ�
        rb = GetComponent<Rigidbody2D>();     // 
    }

    // Update is called once per frame
    void Update()
    {
        PlayerScript(px, psy, pby);
    }

    // ���@�֌W����.
    private void PlayerScript(float speed, float smallJump, float bigJump)
    {
        mode = KeyChack();

        Walk(speed);
        HandleJump(smallJump, bigJump);

        switch (mode)
        {
            case MODE.NONE:
                break;

            case MODE.ATTACK:
                Shot();
                break;

            case MODE.RESET:
                ResetButton();
                break;

            case MODE.DETH:
                P_Deth();
                break;

            case MODE.STOP:
                P_Stop();
                break;
        }
    }

    // �ړ�����.
    private void Walk(float speed)
    {
        // ���L�[.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;  // ������.
        }
        // �E�L�[.
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2( speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false; // �E����.
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // �W�����v���C������.
    private void HandleJump(float smallJump, float bigJump)
    {
        // ��i�W�����v.
        if (Input.GetKeyDown(KeyCode.Space) && (isGround || JumpCount < maxJump))
        {
            Jump(psy);
        }

        // �{�^�������������Ă���Ƃ�.
        if(isJump && Input.GetKey(KeyCode.Space))
        {
            j_HoldTime += Time.deltaTime;  // �������ŃW�����v�͑���.

            if (j_HoldTime < j_HoldDura)
            {
                // ���W�����v�����W�����v�Ɍ����ăW�����v�͂𑝉�.
                float currentJump = Mathf.Lerp(smallJump, bigJump, j_HoldTime / j_HoldDura);
                rb.velocity = new Vector2(rb.velocity.x, currentJump);
            }
        }

        // �{�^���𗣂�����W�����v���I��点��.
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJump = false;
        }
    }

    // �W�����v(1�i��).
    private void Jump(float smallJump)
    {
        // �����W�����v��.
        rb.velocity = new Vector2(rb.velocity.x, smallJump);

        isGround = false;
        isJump = true;
        JumpCount++;
        j_HoldTime = 0;
    }

    // �U��.
    private void Shot()
    {
        // ���@�̌����ɒe��ݒ肷��O��flipX�̏�Ԃ��m�F.
        bool currentFlipX = GetComponent<SpriteRenderer>().flipX;
        
        // �e�̐����ʒu����.
        Vector3 shotpos = transform.position;
        float offset = 0.5f;

        if (currentFlipX)
        {
            shotpos.x -= offset;  // �������̏ꍇ.
        }
        else
        {
            shotpos.x += offset;  // �E�����̏ꍇ.
        }

        // �e�𐶐�.
        GameObject p_shot = Instantiate(bullet, shotpos, Quaternion.identity);
        Bullet bulletScript = p_shot.GetComponent<Bullet>();
        
        // �v���C���[�̌��݂̌�����e�ɐݒ�.
        if (bulletScript != null)
        {
            bulletScript.lr = currentFlipX;
        }

        // SE�Đ�.
        if(shot != null)
        {
            shot.Play();
        }
    }

    // ���Z�b�g�{�^���@�\(���@�̂�).
    public void ResetButton()
    {
        // ���@�̈ʒu�����Z�b�g.
        transform.position = initialPosition;

        // ���x�A�����I�ȗ͂����Z�b�g.
        if (rb != null)
        {
            rb.velocity = Vector3.zero;  // ���x���Z�b�g.
            rb.angularVelocity = 0;      // ��]���x���Z�b�g.
        }

        // �e��Ԃ��������.
        isGround = false;
        isJump = false;
        isDeth = false;
        mode = MODE.NONE;

        // �v���C���[���ĕ\�����A�R���C�_�[��L����
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (collider2D != null) collider2D.enabled = true;

        // �A�j���[�V�����̃��Z�b�g.
        if (anim != null)
        {
            anim.Play("");
        }

        // �f�o�b�O�p.
        Debug.Log("���Z�b�g����");
    }

    // ���E.
    public void P_Deth()
    {
        // �����Ԃ�����.
        FindAnyObjectByType<BloodEffect>().SpawnBlood(transform.position);

        isDeth = true;    // ���S�t���O.

        // �X�v���C�g���\���A�R���C�_�[�𖳌���
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (collider2D != null) collider2D.enabled = false;

        // ���ʉ��Đ�.
        if (deth != null) deth.Play();

        Debug.Log("�����m�F");
        FindAnyObjectByType<GameManager>().SetGameState(Game.GameState.GAME_OVER);
        gameObject.SetActive(false);
    }

    // �|�[�Y�V�X�e��(TODO).
    public void P_Stop()
    {

    }

    // ���S����.
    public void Die()
    {
        P_Deth();
    }

    // ��������(TODO).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �n�ʂɐڐG�������̏���.
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;    // �ڒn����.
            isJump = false;     // �W�����v����.
            JumpCount = 0;      // �W�����v�J�E���g���Z�b�g.
            j_HoldTime = 0.0f;  // �p�����ԃ��Z�b�g.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �G�ɂԂ������玀�S��������.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    // �L�[����.
    MODE KeyChack()
    {
        MODE p_mode = mode;

        switch(true)
        {
            // R�L�[.
            case bool _ when Input.GetKey(KeyCode.R):
                p_mode = MODE.RESET;
                break;

            // Z�L�[.
            case bool _ when Input.GetKeyDown(KeyCode.Z):
                p_mode = MODE.ATTACK;
                break;

            // Q�L�[.
            case bool _ when Input.GetKey(KeyCode.Q):
                p_mode = MODE.DETH;
                break;

            // K�L�[.
            case bool _ when Input.GetKey(KeyCode.K):
                p_mode = MODE.STOP;
                break;

            // �{�^���������ĂȂ��Ƃ�.
            default:
                p_mode = MODE.NONE;
                break;
        }

        return p_mode;
    }
}