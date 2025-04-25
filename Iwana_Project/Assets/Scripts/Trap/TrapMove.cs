using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMove : TrapBase
{
    [Header("子class:TrapMove 設定")]
    [SerializeField, Tooltip("")] private float speed;
    [SerializeField, Tooltip("")] private float angle;
    Vector3 direction;

    private void Start()
    {
        // 角度をラジアンに変換し、方向ベクトルを計算
        float radian = angle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0);
    }
    public override void OnEventTrapEnter()
    {

    }
    public override void OnEventTrapStay()
    {
        // 毎フレーム、方向に基づいて移動
        transform.position += direction * speed * Time.deltaTime;
    }
    public override void OnEventTrapEnd()
    {

    }
}
