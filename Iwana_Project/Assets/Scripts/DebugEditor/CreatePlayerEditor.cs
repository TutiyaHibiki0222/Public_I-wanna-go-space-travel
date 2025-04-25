using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(CreatePlayer), true)]
public class CreatePlayerEditor : Editor
{
    CreatePlayer thisScript;

    private void Awake()
    {
        thisScript = target as CreatePlayer;
    }

    private void OnSceneGUI()
    {
        // ���x���̃J�X�^���X�^�C�����쐬
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 16; // �t�H���g�T�C�Y��ύX
        labelStyle.normal.textColor = Color.white; // �����F�𔒂ɐݒ�
        labelStyle.alignment = TextAnchor.MiddleCenter; // ��������

        // �w�i��ǉ����ĕ��������₷������
        labelStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.55f)); // �w�i�F�𔼓����̍��ɐݒ�

        // �V�[���r���[�Ŋe �X�|�[���|�C���g ��`��
        foreach (var pos in thisScript.spawnPoints)
        {
            Handles.color = new Color(0, 161, 233, 100); // �~�̐F
            float radius = 0.1f * Mathf.Max(pos.transform.localScale.x, pos.transform.localScale.y); // �~�̔��a
            Handles.DrawWireDisc(pos.transform.position, Vector3.forward, radius);

            // �ʒu���ړ��\��
            EditorGUI.BeginChangeCheck();
            Vector3 newTrapPosition = Handles.PositionHandle(pos.transform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pos.transform, "Move �X�|�[���|�C���g");
                pos.transform.position = newTrapPosition;
            }

            // �L�����摜��`��
            if (thisScript.playerPrefab != null)
            {
                // playerPrefab ���� SpriteRenderer ���擾
                SpriteRenderer spriteRenderer = thisScript.playerPrefab.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    // �X�v���C�g���擾
                    Sprite sprite = spriteRenderer.sprite;

                    // �X�v���C�g�̃e�N�X�`�����擾
                    Texture2D texture = sprite.texture;

                    // �X�v���C�g��UV���W�����Ƀe�N�X�`����؂蔲��
                    Rect textureRect = sprite.textureRect;
                    textureRect.x /= texture.width;
                    textureRect.y /= texture.height;
                    textureRect.width /= texture.width;
                    textureRect.height /= texture.height;

                    // ���[���h���W���X�N���[�����W�ɕϊ�
                    Vector3 screenPosition = HandleUtility.WorldToGUIPoint(pos.transform.position);

                    // �Œ�T�C�Y��ݒ�i��: 64x64 �s�N�Z���j
                    float iconSize = 64f; // �\���T�C�Y�i�Œ�l�j

                    // �X�v���C�g�̃A�X�y�N�g����l�����ăT�C�Y���v�Z
                    float aspectRatio = sprite.rect.width / sprite.rect.height;
                    float iconWidth = iconSize * aspectRatio; // �A�X�y�N�g�����ɕ��𒲐�
                    float iconHeight = iconSize; // �����͌Œ�

                    // Rect��ݒ�
                    Rect rect = new Rect(
                        screenPosition.x - iconWidth / 2,
                        screenPosition.y - iconHeight / 2,
                        iconWidth,
                        iconHeight
                    );

                    // �X�v���C�g��GUI��ɕ`��
                    Handles.BeginGUI();
                    GUI.DrawTextureWithTexCoords(rect, texture, textureRect);
                    Handles.EndGUI();
                }
            }

            Vector3 labelPosition = pos.transform.position;
            // �Œ�T�C�Y�Ń��x����\��
            Handles.Label(labelPosition, $"{pos.gameObject.name}", labelStyle);
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