using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /************変数宣言*************/
    [SerializeField] public float bs = 0.0f;         // 弾の速度.

    private GameObject player;      // 
    private Camera M_Camera;        // メインカメラ.
    public bool lr;                 // 方向転換.

    /***************関数****************/
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        M_Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        B_Destroy();
    }

    // 弾を消す処理.
    private void B_Destroy()
    {
        Vector3 screenPoint = M_Camera.WorldToViewportPoint(transform.position);
        bool isOutScreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;

        // 画面外に行ったら消える.
        if (isOutScreen)
        {
            Destroy(gameObject);
        }
    }

    // 壁に当たったら弾を消す処理.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ブロックか敵に当たったら消える.
        if(collision.gameObject.tag == "Ground" ||
           collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    // 左右方向で弾の位置を変える.
    private void FixedUpdate()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

        if (lr)
        {
            rb.velocity = new Vector2(-bs, 0);  // 左向き.
        }
        else
        {
            rb.velocity = new Vector2(bs, 0);   // 右向き.
        }
    }
}