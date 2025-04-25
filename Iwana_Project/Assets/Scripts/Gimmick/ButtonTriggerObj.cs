using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attribute.Add;

public class ButtonTriggerObj : MonoBehaviour
{
    [SerializeField, Tooltip("����������id")] public string id;
    [SerializeField, Tooltip("�^�[�Q�b�g")]   string targetTag = "Bullet";
    [Tooltip("������Trap")] public List<TrapBase> trapList = new List<TrapBase>();

    [Tooltip("�{�^�������ꂽ��")] bool isButton;
    [SerializeField, Tooltip("�摜")] Sprite sprite;
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

    // Start is called before the first frame update
    void Awake()
    {
        isButton = false;
        trapList = GetTrapBase();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isButton) return;
        if (collision.tag != targetTag) { return; }

        GetComponent<SpriteRenderer>().sprite = sprite;
        OnEventTrigger();
    }


    [Button("�g���b�v�ގ��s")]
    private void OnEventTrigger()
    {
        foreach (TrapBase t in trapList)
        {
            t.IsStart = true;
        }
    }
}
