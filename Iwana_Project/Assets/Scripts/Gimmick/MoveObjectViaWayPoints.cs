using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveSetting
{
    [SerializeField, Tooltip("�ړ����x"), Min(0f)] public float moveSpeed = 1f;
    [SerializeField, Tooltip("�ړ���")]            public Vector3 position;
}

public class MoveObjectViaWayPoints : Machine
{
    [SerializeField, Tooltip("�J�n�����������s")] bool isOnStart = true;
    [SerializeField, Tooltip("�ړ���ݒ�")] public List<MoveSetting> moves;
    [SerializeField, Tooltip("reverse")]  public bool isReverse = true;
    [SerializeField, Tooltip("fullLoop")] public bool isFullLoop = false;
    [SerializeField, Tooltip("�J�n�z��ԍ�")] int startIndex;

    [Tooltip("�J�n���W�l")] Vector3 startPos;
    [Tooltip("����Index")] int moveIndex;
    [Tooltip("�����C�x���g")] bool isActive;
    bool startReverse;
    bool startFullLoop;
    List<MoveSetting> initMoves;
    // Start is called before the first frame update
    void Start()
    {
        startPos = gameObject.transform.position;
        moveIndex = startIndex;
        startReverse = isReverse;
        startFullLoop = isFullLoop;
        initMoves = moves;

        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().isKinematic = true;

        if (startIndex < 0)
            startIndex = 0;
        if (startIndex > moves.Count - 1)
            startIndex = moves.Count - 1;

        moveIndex = startIndex;

        if (isOnStart) StartMachine();
    }

    IEnumerator IE_MovePlatform()
    {
        yield return null;
        while (isActive)
        {
            while (Vector2.Distance(transform.position, moves[moveIndex].position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, moves[moveIndex].position, Time.deltaTime * moves[moveIndex].moveSpeed);
                yield return null;
            }

            CalculateWaypoint();

            yield return null;
        }
    }

    void CalculateWaypoint()
    {
        if (isReverse)
        {
            if (moveIndex + 1 >= moves.Count)
            {
                if (isFullLoop)
                    moveIndex = 0;
                else
                {
                    isReverse = false;
                    moveIndex--;
                }
            }
            else
                moveIndex++;

        }
        else
        {
            if (moveIndex - 1 < 0)
            {
                if (isFullLoop)
                    moveIndex = (moves.Count) - 1;
                else
                {
                    isReverse = true;
                    moveIndex++;
                }
            }
            else
                moveIndex--;
        }
    }



    public override void StartMachine()
    {
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(IE_MovePlatform());
        }
    }

    public override void StopMachine()
    {
        if (isActive)
        {
            StopAllCoroutines();
            isActive = false;
        }
    }

    public override void ReverseMachine()
    {
        if (isReverse)
            isReverse = false;
        else
            isReverse = true;
        CalculateWaypoint();
    }

    public override void ResetMachine()
    {
        gameObject.transform.position = startPos;
        moveIndex = startIndex;
        isReverse = startReverse;
        isFullLoop = startFullLoop;
        moves = initMoves;

        isActive = false;
        StopAllCoroutines();
    }
}
