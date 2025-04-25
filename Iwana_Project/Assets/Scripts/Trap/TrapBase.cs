using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class TrapConfig
{
    [SerializeField, Tooltip("TrapID")]     public string id;
    [SerializeField, Tooltip("IsDestroy")]  public bool isDestroy = true;
    [SerializeField, Tooltip("íœŽžŠÔ")]   public float destroyTime;
}


public abstract class TrapBase : MonoBehaviour
{
    [Header("TrapBase Ý’è")]
    [SerializeField, Tooltip("TrapID")] TrapConfig trapConfig;
    private bool isStart;

    public bool IsStart
    {
        get { return isStart; }
        set { isStart = value; }
    }
    private bool isEnter;

    public string TrapID => trapConfig.id;

    void Awake()
    {
        print("Trap Awake");
        isStart = false;
        isEnter = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            if (isEnter)
            {
                isEnter = false;
                OnEventTrapEnter();
                OnEventTrapStay();
                if (trapConfig.isDestroy) StartCoroutine(C_DestroyTime());
            }
            else
            {
                OnEventTrapStay();
            }
        }
    }

    public virtual void OnEventTrapEnter()
    {

    }

    public virtual void OnEventTrapStay()
    {

    }

    public virtual void OnEventTrapEnd()
    {

    }

    IEnumerator C_DestroyTime()
    {
        yield return new WaitForSeconds(trapConfig.destroyTime);
        OnEventTrapEnd();

        Destroy(gameObject);
    }

    public List<Trigger> GetTriggers()
    {
        List<Trigger> triggers = new List<Trigger>();
        var list = FindObjectsOfType<Trigger>();
        foreach (var trigger in list)
        {
            if(TrapID == trigger.id)
            {
                triggers.Add(trigger);
            }
        }
        return triggers;
    }

}
