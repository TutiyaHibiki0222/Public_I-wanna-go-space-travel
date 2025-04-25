using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerConfig
{
    [SerializeField, Tooltip("�����蔻��Size")] public Vector2 size = new Vector2(1,1);
    [SerializeField, Tooltip("���S�ɂ��邩")]   public bool isCenter = false;
    [SerializeField, Tooltip("���͈̔͂��g��")] public float x = 0f;
    [SerializeField, Tooltip("�c�͈̔͂��g��")] public float y = 0f;
}

public class Trigger : MonoBehaviour
{
    [SerializeField, Tooltip("����������id")]       public string id;
    [SerializeField, Tooltip("�����蔻��͈͐ݒ�")] public TriggerConfig trigger;
    [SerializeField, Tooltip("�������锻��")] bool isEnter, isExit;
    [SerializeField, Tooltip("�^�[�Q�b�g")] string targetTag;
    [Tooltip("������Trap")] public List<TrapBase> trapList = new List<TrapBase>();
    private BoxCollider2D box;

    public List<TrapBase> GetTrapBase()
    {
        List<TrapBase> trapBases = new List<TrapBase>();
        var list = FindObjectsOfType<TrapBase>();
        foreach (TrapBase t in list)
        {
            if (t.TrapID == id)
            {
                trapBases.Add(t);
            }
        }
        return trapBases;
    }


    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        var list = FindObjectsOfType<TrapBase>();
        trapList = GetTrapBase();
        box = GetComponent<BoxCollider2D>();
        if (!box)
        {
            box = gameObject.AddComponent<BoxCollider2D>();
        }
        box.isTrigger = true;
        // �����蔻��͈͂̐ݒ�
        ConfigureCollider();
    }

    private void ConfigureCollider()
    {
        box.size = trigger.size;

        // ���݂�BoxCollider2D�T�C�Y
        Vector2 originalSize = box.size;

        // �T�C�Y���g��
        box.size = new Vector2(
            originalSize.x + Mathf.Abs(trigger.x),
            originalSize.y + Mathf.Abs(trigger.y)
        );

        // �I�t�Z�b�g�̐ݒ�
        box.offset = trigger.isCenter ? Vector2.zero : new Vector2(trigger.x / 2, trigger.y / 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isEnter) { return; }
        if(collision.tag != targetTag) { return; }
        StratEventTrap();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isExit) { return; }
        if (collision.tag != targetTag) { return; }
        StratEventTrap();
    }

    public void StratEventTrap()
    {
        foreach (TrapBase t in trapList)
        {
            t.IsStart = true;
        }
    }

}
