using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(ButtonTriggerObj),true)]
public class ButtonTriggerObjEditor : Editor
{
    ButtonTriggerObj thisScript;
    void Awake()
    {
        thisScript = target as ButtonTriggerObj;
    }

    private void OnSceneGUI()
    {
        var traps = thisScript.GetTrapBase();

        // TrapBase ���Ȃ��ꍇ�͉������Ȃ�
        if (traps == null || traps.Count == 0) return;

        // ���x���̃J�X�^���X�^�C�����쐬
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 16; // �t�H���g�T�C�Y��ύX
        labelStyle.normal.textColor = Color.white; // �����F�𔒂ɐݒ�
        labelStyle.alignment = TextAnchor.MiddleCenter; // ��������

        // �w�i��ǉ����ĕ��������₷������
        labelStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.55f)); // �w�i�F�𔼓����̍��ɐݒ�

        // �V�[���r���[�Ŋe TrapBase ��`��
        foreach (var trap in traps)
        {
            if (trap != null)
            {
                Handles.DrawLine(thisScript.transform.position, trap.transform.position);
                // ��������`��
                Handles.color = new Color(0, 161, 233);
                float lineWidth = 10.0f; // ���̑���
                Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    thisScript.transform.position,
                    trap.transform.position
                });

                Handles.color = new Color(0, 161, 233, 100); // �~�̐F
                float radius = 0.25f * Mathf.Max(trap.transform.localScale.x, trap.transform.localScale.y); // �~�̔��a
                Handles.DrawWireDisc(trap.transform.position, Vector3.forward, radius);

                // TrapBase �̈ʒu���ړ��\��
                EditorGUI.BeginChangeCheck();
                Vector3 newTrapPosition = Handles.PositionHandle(trap.transform.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(trap.transform, "Move TrapBase");
                    trap.transform.position = newTrapPosition;
                }

                // TrapBase �̈ʒu�Ƀ��x����\��
                // �J�����̋����Ɉˑ����Ȃ��T�C�Y�ɌŒ肷�邽�߂ɁA�\���T�C�Y���Œ�
                Vector3 labelPosition = trap.transform.position;
                // �Œ�T�C�Y�Ń��x����\��
                Handles.Label(labelPosition, $"Trap: {trap.TrapID}", labelStyle);
            }
        }
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
}
#endif