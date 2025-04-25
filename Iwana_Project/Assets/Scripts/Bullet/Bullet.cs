using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /************�ϐ��錾*************/
    [SerializeField] public float bs = 0.0f;         // �e�̑��x.

    private GameObject player;      // 
    private Camera M_Camera;        // ���C���J����.
    public bool lr;                 // �����]��.

    /***************�֐�****************/
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

    // �e����������.
    private void B_Destroy()
    {
        Vector3 screenPoint = M_Camera.WorldToViewportPoint(transform.position);
        bool isOutScreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;

        // ��ʊO�ɍs�����������.
        if (isOutScreen)
        {
            Destroy(gameObject);
        }
    }

    // �ǂɓ���������e����������.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �u���b�N���G�ɓ��������������.
        if(collision.gameObject.tag == "Ground" ||
           collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    // ���E�����Œe�̈ʒu��ς���.
    private void FixedUpdate()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

        if (lr)
        {
            rb.velocity = new Vector2(-bs, 0);  // ������.
        }
        else
        {
            rb.velocity = new Vector2(bs, 0);   // �E����.
        }
    }
}