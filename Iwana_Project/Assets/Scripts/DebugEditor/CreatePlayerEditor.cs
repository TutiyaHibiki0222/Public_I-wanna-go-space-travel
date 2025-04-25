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
        // ラベルのカスタムスタイルを作成
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 16; // フォントサイズを変更
        labelStyle.normal.textColor = Color.white; // 文字色を白に設定
        labelStyle.alignment = TextAnchor.MiddleCenter; // 中央揃え

        // 背景を追加して文字を見やすくする
        labelStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.55f)); // 背景色を半透明の黒に設定

        // シーンビューで各 スポーンポイント を描画
        foreach (var pos in thisScript.spawnPoints)
        {
            Handles.color = new Color(0, 161, 233, 100); // 円の色
            float radius = 0.1f * Mathf.Max(pos.transform.localScale.x, pos.transform.localScale.y); // 円の半径
            Handles.DrawWireDisc(pos.transform.position, Vector3.forward, radius);

            // 位置を移動可能に
            EditorGUI.BeginChangeCheck();
            Vector3 newTrapPosition = Handles.PositionHandle(pos.transform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pos.transform, "Move スポーンポイント");
                pos.transform.position = newTrapPosition;
            }

            // キャラ画像を描画
            if (thisScript.playerPrefab != null)
            {
                // playerPrefab から SpriteRenderer を取得
                SpriteRenderer spriteRenderer = thisScript.playerPrefab.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    // スプライトを取得
                    Sprite sprite = spriteRenderer.sprite;

                    // スプライトのテクスチャを取得
                    Texture2D texture = sprite.texture;

                    // スプライトのUV座標を元にテクスチャを切り抜く
                    Rect textureRect = sprite.textureRect;
                    textureRect.x /= texture.width;
                    textureRect.y /= texture.height;
                    textureRect.width /= texture.width;
                    textureRect.height /= texture.height;

                    // ワールド座標をスクリーン座標に変換
                    Vector3 screenPosition = HandleUtility.WorldToGUIPoint(pos.transform.position);

                    // 固定サイズを設定（例: 64x64 ピクセル）
                    float iconSize = 64f; // 表示サイズ（固定値）

                    // スプライトのアスペクト比を考慮してサイズを計算
                    float aspectRatio = sprite.rect.width / sprite.rect.height;
                    float iconWidth = iconSize * aspectRatio; // アスペクト比を基に幅を調整
                    float iconHeight = iconSize; // 高さは固定

                    // Rectを設定
                    Rect rect = new Rect(
                        screenPosition.x - iconWidth / 2,
                        screenPosition.y - iconHeight / 2,
                        iconWidth,
                        iconHeight
                    );

                    // スプライトをGUI上に描画
                    Handles.BeginGUI();
                    GUI.DrawTextureWithTexCoords(rect, texture, textureRect);
                    Handles.EndGUI();
                }
            }

            Vector3 labelPosition = pos.transform.position;
            // 固定サイズでラベルを表示
            Handles.Label(labelPosition, $"{pos.gameObject.name}", labelStyle);
        }
    }

    // 背景用のテクスチャを作成するメソッド
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