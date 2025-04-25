using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodObj : MonoBehaviour
{

    BoxCollider2D box;
    Rigidbody2D rb;
    Vector2 cmin;
    Vector2 cmax;
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        // �J�����͈̔͂��擾
        cmin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        cmax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        float size = Random.Range(0.05f, 0.075f);
        transform.localScale = new Vector3(size, size,1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        // �J�����̊O�ɏo����
        if (pos.x < cmin.x - 1|| pos.y < cmin.y - 1 || pos.x > cmax.x + 1 || pos.y > cmax.y + 1)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            rb.velocity = Vector2.zero;             // ���̑��x���~�߂�
            rb.gravityScale = 0;                    // �d�͂𖳌���
            rb.bodyType = RigidbodyType2D.Static;   // Rigidbody2D ��ÓI�ɕύX
            rb.simulated = false;                   // �Փ˔���𖳌����i�K�v�ɉ����āj
        }
    }
}
