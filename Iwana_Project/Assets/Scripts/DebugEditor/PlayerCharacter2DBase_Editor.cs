#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayerCharacter2DBase), true)]
public class PlayerCharacter2DBase_Editor : Editor
{
    PlayerCharacter2DBase thisScript;
    private void Awake()
    {
        thisScript = (PlayerCharacter2DBase)target;
        thisScript.SetMinPenetration();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("X_Speed: ", "" + thisScript.X_Speed * 100);
        EditorGUILayout.LabelField("F_Speed: ", "" + thisScript.Y_Speed * 2);
        EditorGUILayout.LabelField("");

        EditorGUILayout.LabelField("GravityMultiplier: ", "" + thisScript.GravityMultiplier);
        EditorGUILayout.LabelField("MovementMultiplier: ", "" + thisScript.MovementMultiplier);
        EditorGUILayout.LabelField("JumpforceMultiplier: ", "" + thisScript.JumpforceMultiplier);

    }
}
#endif