using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    /************変数宣言*************/
    [SerializeField] private float px = 0.0f;               // 通常移動.
    [SerializeField] private float psy = 0.0f;              // 小ジャンプ.
    [SerializeField] private float pby = 0.0f;              // 大ジャンプ.
    [SerializeField] private float j_HoldTime = 0.0f;       // ジャンプボタンを押し続けた時間.
    [SerializeField] private float j_HoldDura = 0.0f;       // 最大ジャンプ継続時間.
    [SerializeField] private int maxJump = 2;               // 二段ジャンプ.
    [SerializeField] private bool isGround;                 // 接地判定.
    [SerializeField] private bool isJump;                   // ジャンプ判定.
    [SerializeField] private bool isDeth;                   // 死亡判定.
    [SerializeField] GameObject bullet;                     // 弾のセット.
    //[SerializeField] GameObject dethPrefab;                 // 血しぶきのプレファブ.
    [SerializeField] private Vector3 initialPosition;       // 初期位置保存.
    [SerializeField] private AudioSource jump;              // ジャンプSE.
    [SerializeField] private AudioSource deth;              // 死亡SE.
    [SerializeField] private AudioSource shot;              // ショットSE.
    [SerializeField] private SpriteRenderer spriteRenderer; // プレイヤーのスプライト
    [SerializeField] private new Collider2D collider2D;     // プレイヤーのコライダー
    [SerializeField] private Transform respownPoint;        // リスポーン地点.

    private Animator anim;               // アニメーション.
    private Rigidbody2D rb;              // 当たり判定.
    private int JumpCount = 0;           // 現在のジャンプ回数.

    // 自機のモード.
    enum MODE{ NONE, ATTACK, RESET, DETH, STOP };
    private MODE mode = MODE.NONE;

    /***************関数****************/
    // Start is called before the first frame update
    void Start()
    {
        mode = MODE.NONE;                     // 初期モード.
        initialPosition = transform.position; // プレイヤーの初期位置を保存
        rb = GetComponent<Rigidbody2D>();     // 
    }

    // Update is called once per frame
    void Update()
    {
        PlayerScript(px, psy, pby);
    }

    // 自機関係処理.
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

    // 移動処理.
    private void Walk(float speed)
    {
        // 左キー.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;  // 左向き.
        }
        // 右キー.
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2( speed, rb.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false; // 右向き.
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // ジャンプメイン処理.
    private void HandleJump(float smallJump, float bigJump)
    {
        // 一段ジャンプ.
        if (Input.GetKeyDown(KeyCode.Space) && (isGround || JumpCount < maxJump))
        {
            Jump(psy);
        }

        // ボタンを押し続けているとき.
        if(isJump && Input.GetKey(KeyCode.Space))
        {
            j_HoldTime += Time.deltaTime;  // 長押しでジャンプ力増加.

            if (j_HoldTime < j_HoldDura)
            {
                // 小ジャンプから大ジャンプに向けてジャンプ力を増加.
                float currentJump = Mathf.Lerp(smallJump, bigJump, j_HoldTime / j_HoldDura);
                rb.velocity = new Vector2(rb.velocity.x, currentJump);
            }
        }

        // ボタンを離したらジャンプを終わらせる.
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJump = false;
        }
    }

    // ジャンプ(1段目).
    private void Jump(float smallJump)
    {
        // 初期ジャンプ力.
        rb.velocity = new Vector2(rb.velocity.x, smallJump);

        isGround = false;
        isJump = true;
        JumpCount++;
        j_HoldTime = 0;
    }

    // 攻撃.
    private void Shot()
    {
        // 自機の向きに弾を設定する前にflipXの状態を確認.
        bool currentFlipX = GetComponent<SpriteRenderer>().flipX;
        
        // 弾の生成位置調整.
        Vector3 shotpos = transform.position;
        float offset = 0.5f;

        if (currentFlipX)
        {
            shotpos.x -= offset;  // 左向きの場合.
        }
        else
        {
            shotpos.x += offset;  // 右向きの場合.
        }

        // 弾を生成.
        GameObject p_shot = Instantiate(bullet, shotpos, Quaternion.identity);
        Bullet bulletScript = p_shot.GetComponent<Bullet>();
        
        // プレイヤーの現在の向きを弾に設定.
        if (bulletScript != null)
        {
            bulletScript.lr = currentFlipX;
        }

        // SE再生.
        if(shot != null)
        {
            shot.Play();
        }
    }

    // リセットボタン機能(自機のみ).
    public void ResetButton()
    {
        // 自機の位置をリセット.
        transform.position = initialPosition;

        // 速度、物理的な力をリセット.
        if (rb != null)
        {
            rb.velocity = Vector3.zero;  // 速度リセット.
            rb.angularVelocity = 0;      // 回転速度リセット.
        }

        // 各状態を初期状態.
        isGround = false;
        isJump = false;
        isDeth = false;
        mode = MODE.NONE;

        // プレイヤーを再表示し、コライダーを有効化
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (collider2D != null) collider2D.enabled = true;

        // アニメーションのリセット.
        if (anim != null)
        {
            anim.Play("");
        }

        // デバッグ用.
        Debug.Log("リセット完了");
    }

    // 自殺.
    public void P_Deth()
    {
        // 血しぶき生成.
        FindAnyObjectByType<BloodEffect>().SpawnBlood(transform.position);

        isDeth = true;    // 死亡フラグ.

        // スプライトを非表示、コライダーを無効化
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (collider2D != null) collider2D.enabled = false;

        // 効果音再生.
        if (deth != null) deth.Play();

        Debug.Log("死去確認");
        FindAnyObjectByType<GameManager>().SetGameState(Game.GameState.GAME_OVER);
        gameObject.SetActive(false);
    }

    // ポーズシステム(TODO).
    public void P_Stop()
    {

    }

    // 死亡処理.
    public void Die()
    {
        P_Deth();
    }

    // 物理判定(TODO).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面に接触した時の処理.
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;    // 接地判定.
            isJump = false;     // ジャンプ判定.
            JumpCount = 0;      // ジャンプカウントリセット.
            j_HoldTime = 0.0f;  // 継続時間リセット.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵にぶつかったら死亡処理発動.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    // キー判定.
    MODE KeyChack()
    {
        MODE p_mode = mode;

        switch(true)
        {
            // Rキー.
            case bool _ when Input.GetKey(KeyCode.R):
                p_mode = MODE.RESET;
                break;

            // Zキー.
            case bool _ when Input.GetKeyDown(KeyCode.Z):
                p_mode = MODE.ATTACK;
                break;

            // Qキー.
            case bool _ when Input.GetKey(KeyCode.Q):
                p_mode = MODE.DETH;
                break;

            // Kキー.
            case bool _ when Input.GetKey(KeyCode.K):
                p_mode = MODE.STOP;
                break;

            // ボタンを押してないとき.
            default:
                p_mode = MODE.NONE;
                break;
        }

        return p_mode;
    }
}