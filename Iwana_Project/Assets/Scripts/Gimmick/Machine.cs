using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Machine : MonoBehaviour
{
    public abstract void StartMachine();
    public abstract void StopMachine();
    public abstract void ResetMachine();

    public abstract void ReverseMachine();
}
