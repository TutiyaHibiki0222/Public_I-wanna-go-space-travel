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

        // TrapBase ���Ȃ��ꍇ�͉������Ȃ�
        if (trgs == null || trgs.Count == 0) return;

        // �V�[���r���[�Ŋe TrapBase ��`��
        foreach (var trg in trgs)
        {
            if (trg != null)
            {

                // ��������`��
                Handles.color = new Color(0, 161, 233);
                float lineWidth = 10.0f; // ���̑���
                Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    thisScript.transform.position,
                    trg.transform.position
                    });
                // TrapBase �̈ʒu���ړ��\��
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
