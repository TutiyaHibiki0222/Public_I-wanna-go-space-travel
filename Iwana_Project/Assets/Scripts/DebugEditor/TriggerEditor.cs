#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Trigger), true)]
public class TriggerEditor : Editor
{
    Trigger thisScript;
    void Awake()
    {
        thisScript = target as Trigger;
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


        {
            TriggerConfig t = thisScript.trigger;

            // Transform �̃X�P�[�����擾
            Vector3 baseSize = thisScript.transform.localScale;

            // BoxCollider2D �̃T�C�Y�v�Z
            Vector3 size = Vector3.Scale(baseSize, t.size); // �����蔻��̃T�C�Y����
            size.x += Mathf.Abs(t.x) * baseSize.x; // x������1�T�C�Y���g��
            size.y += Mathf.Abs(t.y) * baseSize.y; // y������1�T�C�Y���g��

            // BoxCollider2D �̃I�t�Z�b�g�v�Z�iisCenter��false�̏ꍇ�A�g��������1�}�X�����炷�j
            Vector3 offset = Vector3.zero;
            if (!t.isCenter)
            {
                if (t.x != 0) offset.x = (t.x / 2) * baseSize.x; // x������1�}�X�����炷
                if (t.y != 0) offset.y = (t.y / 2) * baseSize.y; // y������1�}�X�����炷
            }

            // ���ۂ̈ʒu
            Vector3 pos = thisScript.transform.position + offset;


            // �l�p�`�̒��_���v�Z
            Vector3[] rectPoints =
            {
                pos + new Vector3(-size.x / 2, -size.y / 2, 0), // ����
                pos + new Vector3( size.x / 2, -size.y / 2, 0), // �E��
                pos + new Vector3( size.x / 2,  size.y / 2, 0), // �E��
                pos + new Vector3(-size.x / 2,  size.y / 2, 0), // ����
                pos + new Vector3(-size.x / 2, -size.y / 2, 0)  // �����i����j
            };

            // Scene�r���[�ɘg����`��
            Handles.color = Color.green;
            Handles.DrawPolyLine(rectPoints);
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


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Editor���Trap��Debug�Ŕ��������p�̃{�^���ł��B");

        // StratEventTrap() �����s����{�^�����쐬
        if (GUILayout.Button("Start Event Trap"))
        {
            thisScript.StratEventTrap();
        }
    }
}
#endif