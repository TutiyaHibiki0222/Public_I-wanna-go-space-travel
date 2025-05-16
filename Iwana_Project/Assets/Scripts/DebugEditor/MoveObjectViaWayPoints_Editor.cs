#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveObjectViaWayPoints))]
public class MoveObjectViaWayPoints_Editor : Editor
{
    MoveObjectViaWayPoints thisScript;
    Machine machine;
    private void OnSceneGUI()
    {
        thisScript = (MoveObjectViaWayPoints)target;

        if (thisScript.moves == null || thisScript.moves.Count < 2) return;

        Handles.color = Color.yellow;
        // ���x���̃J�X�^���X�^�C�����쐬
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white; // �����F�𔒂ɐݒ�
        // �w�i��ǉ����ĕ��������₷������
        labelStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.55f)); // �w�i�F�𔼓����̍��ɐݒ�


        for (int i = 0; i < thisScript.moves.Count; i++)
        {
            Vector3 point = thisScript.moves[i].position;

            if (point != null)  // `null` �`�F�b�N�͕s�v�B`point` �� `Vector3` �^�Ȃ̂�
            {
                // �|�C���g�̈ړ�
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.PositionHandle(point, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    // Undo�������L�^
                    Undo.RecordObject(thisScript, "Move Patrol Point");
                    thisScript.moves[i].position = newPosition;  // moves[i] �� position ���X�V
                }

                // �~��`��
                Handles.DrawWireDisc(point, Vector3.forward, 0.01f);
            }
            // ����`��
            if (thisScript.moves.Count - 1 < 2 && i < thisScript.moves.Count - 1 && thisScript.moves[i + 1] != null)
            {
                DrawArrowToArrow(point, thisScript.moves[i + 1].position, Color.yellow);
            }
            else if (i < thisScript.moves.Count - 1 && thisScript.moves[i + 1] != null)
            {
                if (thisScript.isReverse) DrawArrow(point, thisScript.moves[i + 1].position, Color.yellow);
                else DrawArrow(thisScript.moves[i + 1].position, point, Color.yellow);
            }
            else if (thisScript.moves.Count >= 3 && thisScript.isFullLoop)
            {
                if (thisScript.isReverse) DrawArrow(point, thisScript.moves[0].position, Color.red);
                else DrawArrow(thisScript.moves[0].position, point, Color.red);
            }

            // ���x��
            Handles.Label(point, $"Move Point: {i}", labelStyle);
        }
    }
    // ���݂��̕�����`�悷�郁�\�b�h.
    public void DrawArrowToArrow(Vector3 start, Vector3 end, Color color)
    {
        Handles.color = color;

        // ���̕����ƒ���
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // ���̏I�_����O�ɂ��炷
        float offset = 0.25f;
        Vector3 adjustedStart = end - direction * (distance - offset);
        Vector3 adjustedEnd = start + direction * (distance - offset);

        float lineWidth = 10.0f; // ���̑���
        Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    adjustedStart - direction * (offset * 0.75f),
                    adjustedEnd + direction * (offset * 0.75f)
                });
        // ���̕`��
        Handles.DrawLine(adjustedStart, adjustedEnd);
        // ���̕`��
        float arrowSize = 0.2f; // ���̃T�C�Y
        Handles.ArrowHandleCap(
            0,
            adjustedEnd, // �M���M����O�̈ʒu
            Quaternion.LookRotation( direction), // ���̌���
            arrowSize,
            EventType.Repaint
        );
        Handles.ArrowHandleCap(
            0,
            adjustedStart, // �M���M����O�̈ʒu
            Quaternion.LookRotation(-direction), // ���̌���
            arrowSize,
            EventType.Repaint
        );
    }
    // ����`�悷�郁�\�b�h
    private void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        Handles.color = color;

        // ���̕����ƒ���
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // ���̏I�_����O�ɂ��炷
        float offset = 0.25f; // �I���_��O�̃I�t�Z�b�g��
        Vector3 adjustedEnd = start + direction * (distance - offset);
        
        float lineWidth = 10.0f; // ���̑���
        Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    start,
                    adjustedEnd + direction * (offset * 0.75f)
                });

        // ���̕`��
        Handles.DrawLine(start, adjustedEnd);

        // ���̕`��
        float arrowSize = 0.2f; // ���̃T�C�Y
        Handles.ArrowHandleCap(
            0,
            adjustedEnd, // �M���M����O�̈ʒu
            Quaternion.LookRotation(direction), // ���̌���
            arrowSize,
            EventType.Repaint
        );
    }

    // �w�i�p�̃e�N�X�`�����쐬���郁�\�b�h
    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = color;
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(pix);
        tex.Apply();
        return tex;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        machine = (Machine)target;
        EditorGUILayout.LabelField("MoveObjectViaWayPoints �p�J�X�^���ݒ�", EditorStyles.boldLabel);
        
        // �h���b�O�A���h�h���b�v��t�p�̗̈���쐬
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "������ GameObject ���h���b�O���h���b�v", EditorStyles.helpBox);

        // �h���b�O����
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Undo.RecordObject(machine, "Add Move Points");

                    foreach (Object draggedObj in DragAndDrop.objectReferences)
                    {
                        if (draggedObj is GameObject go)
                        {
                            MoveSetting newSetting = new MoveSetting
                            {
                                position = go.transform.position,
                                moveSpeed = 1f
                            };
                            ((MoveObjectViaWayPoints)target).moves.Add(newSetting);
                        }
                    }

                    EditorUtility.SetDirty(machine);
                }

                evt.Use();
                break;
        }

        if (GUILayout.Button("Start_Machine"))
        {
            machine.StartMachine();
        }

        if (GUILayout.Button("Stop_Machine"))
        {
            machine.StopMachine();
        }

        if (GUILayout.Button("Reset_Machine"))
        {
            machine.ResetMachine();
        }

        if (GUILayout.Button("Reverse_Machine"))
        {
            machine.ReverseMachine();
        }
    }
}
#endif
