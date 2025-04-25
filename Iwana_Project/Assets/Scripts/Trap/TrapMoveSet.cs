using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class MoveSet 
{
    [SerializeField, Tooltip("���x")]    public float speed = 3;
    [SerializeField, Tooltip("�ړ���.")] public Vector3 position;
    [Min(0)]
    [SerializeField, Tooltip("�ҋ@����.")] public float time;
}



public class TrapMoveSet : TrapBase
{
    [SerializeField, Tooltip("Loop")] bool isLoop;
    [SerializeField, Tooltip("�g���b�v�ړ���")] List<MoveSet> moveSet;
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
                // ���݂�point_s�̈ʒu���X�V
                Vector3 point_Pos = moveSet[i].position;
                if ((point_Pos - transform.position).sqrMagnitude > Mathf.Epsilon)
                {
                    //moveObj�̃|�W�V������ύX
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
