using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMove : TrapBase
{
    [Header("�qclass:TrapMove �ݒ�")]
    [SerializeField, Tooltip("")] private float speed;
    [SerializeField, Tooltip("")] private float angle;
    Vector3 direction;

    private void Start()
    {
        // �p�x�����W�A���ɕϊ����A�����x�N�g�����v�Z
        float radian = angle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0);
    }
    public override void OnEventTrapEnter()
    {

    }
    public override void OnEventTrapStay()
    {
        // ���t���[���A�����Ɋ�Â��Ĉړ�
        transform.position += direction * speed * Time.deltaTime;
    }
    public override void OnEventTrapEnd()
    {

    }
}
