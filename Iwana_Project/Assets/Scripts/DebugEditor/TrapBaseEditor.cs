#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TrapBase), true)]
public class TrapBaseEditor : Editor
{
    TrapBase thisScript;
    void Awake()
    {
        thisScript = target as TrapBase;
    }

    private void OnSceneGUI()
    {
        var trgs = thisScript.GetTriggers();

        // TrapBase がない場合は何もしない
        if (trgs == null || trgs.Count == 0) return;

        // シーンビューで各 TrapBase を描画
        foreach (var trg in trgs)
        {
            if (trg != null)
            {

                // 太い線を描画
                Handles.color = new Color(0, 161, 233);
                float lineWidth = 10.0f; // 線の太さ
                Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    thisScript.transform.position,
                    trg.transform.position
                    });
                // TrapBase の位置を移動可能に
                EditorGUI.BeginChangeCheck();
                Vector3 newTrapPosition = Handles.PositionHandle(trg.transform.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(trg.transform, "Trigger ");
                    trg.transform.position = newTrapPosition;
                }
            }
        }

    }
}
#endif
