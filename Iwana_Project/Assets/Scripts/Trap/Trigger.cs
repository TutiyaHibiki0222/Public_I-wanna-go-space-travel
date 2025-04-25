using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerConfig
{
    [SerializeField, Tooltip("当たり判定Size")] public Vector2 size = new Vector2(1,1);
    [SerializeField, Tooltip("中心にするか")]   public bool isCenter = false;
    [SerializeField, Tooltip("横の範囲を拡張")] public float x = 0f;
    [SerializeField, Tooltip("縦の範囲を拡張")] public float y = 0f;
}

public class Trigger : MonoBehaviour
{
    [SerializeField, Tooltip("動かしたいid")]       public string id;
    [SerializeField, Tooltip("当たり判定範囲設定")] public TriggerConfig trigger;
    [SerializeField, Tooltip("発生する判定")] bool isEnter, isExit;
    [SerializeField, Tooltip("ターゲット")] string targetTag;
    [Tooltip("動かすTrap")] public List<TrapBase> trapList = new List<TrapBase>();
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
        // 当たり判定範囲の設定
        ConfigureCollider();
    }

    private void ConfigureCollider()
    {
        box.size = trigger.size;

        // 現在のBoxCollider2Dサイズ
        Vector2 originalSize = box.size;

        // サイズを拡張
        box.size = new Vector2(
            originalSize.x + Mathf.Abs(trigger.x),
            originalSize.y + Mathf.Abs(trigger.y)
        );

        // オフセットの設定
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
