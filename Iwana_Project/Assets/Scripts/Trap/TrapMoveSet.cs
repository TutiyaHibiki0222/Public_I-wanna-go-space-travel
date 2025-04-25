using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class MoveSet 
{
    [SerializeField, Tooltip("速度")]    public float speed = 3;
    [SerializeField, Tooltip("移動先.")] public Vector3 position;
    [Min(0)]
    [SerializeField, Tooltip("待機時間.")] public float time;
}



public class TrapMoveSet : TrapBase
{
    [SerializeField, Tooltip("Loop")] bool isLoop;
    [SerializeField, Tooltip("トラップ移動先")] List<MoveSet> moveSet;
    // Start is called before the first frame update
    public override void OnEventTrapEnter()
    {
        StartCoroutine(MoveStart());
    }

    IEnumerator MoveStart()
    {
        for (int i = 0; i < moveSet.Count; i++)
        {
            while (true)
            {
                // 現在のpoint_sの位置を更新
                Vector3 point_Pos = moveSet[i].position;
                if ((point_Pos - transform.position).sqrMagnitude > Mathf.Epsilon)
                {
                    //moveObjのポジションを変更
                    transform.position =
                        Vector3.MoveTowards(transform.position, point_Pos, moveSet[i].speed * Time.deltaTime);

                }
                else
                {
                    if(moveSet[i].time > 0)
                    {
                        yield return new WaitForSeconds(moveSet[i].time);
                    }
                    break;
                }
                yield return null;
            }
        }
        if (isLoop)
        {
            StartCoroutine(MoveStart());
        }

        yield break;
    }
}
