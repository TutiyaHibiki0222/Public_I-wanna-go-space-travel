#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Machine), true)]
public class MachineEditor : Editor
{
    Machine thisScript;
    [SerializeField] private float speedChange = 1;

    void Awake()
    {
        thisScript = target as Machine;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("デバック 用 button");

        if (GUILayout.Button("Start_Machine"))
        {
            thisScript.StartMachine();
        }

        if (GUILayout.Button("Stop_Machine"))
        {
            thisScript.StopMachine();
        }

        if (GUILayout.Button("Reset_Machine"))
        {
            thisScript.ResetMachine();
        }

        if (GUILayout.Button("Reverse_Machine"))
        {
            thisScript.ReverseMachine();
        }
        
    }
}
#endif