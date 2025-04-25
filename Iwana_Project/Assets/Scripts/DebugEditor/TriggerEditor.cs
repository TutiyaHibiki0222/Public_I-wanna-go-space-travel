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

        // TrapBase がない場合は何もしない
        if (traps == null || traps.Count == 0) return;

        // ラベルのカスタムスタイルを作成
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 16; // フォントサイズを変更
        labelStyle.normal.textColor = Color.white; // 文字色を白に設定
        labelStyle.alignment = TextAnchor.MiddleCenter; // 中央揃え

        // 背景を追加して文字を見やすくする
        labelStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.55f)); // 背景色を半透明の黒に設定

        // シーンビューで各 TrapBase を描画
        foreach (var trap in traps)
        {
            if (trap != null)
            {
                Handles.DrawLine(thisScript.transform.position, trap.transform.position);
                // 太い線を描画
                Handles.color = new Color(0, 161, 233);
                float lineWidth = 10.0f; // 線の太さ
                Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    thisScript.transform.position,
                    trap.transform.position
                });

                Handles.color = new Color(0, 161, 233, 100); // 円の色
                float radius = 0.25f * Mathf.Max(trap.transform.localScale.x, trap.transform.localScale.y); // 円の半径
                Handles.DrawWireDisc(trap.transform.position, Vector3.forward, radius);

                // TrapBase の位置を移動可能に
                EditorGUI.BeginChangeCheck();
                Vector3 newTrapPosition = Handles.PositionHandle(trap.transform.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(trap.transform, "Move TrapBase");
                    trap.transform.position = newTrapPosition;
                }

                // TrapBase の位置にラベルを表示
                // カメラの距離に依存しないサイズに固定するために、表示サイズを固定
                Vector3 labelPosition = trap.transform.position;
                // 固定サイズでラベルを表示
                Handles.Label(labelPosition, $"Trap: {trap.TrapID}", labelStyle);
            }
        }


        {
            TriggerConfig t = thisScript.trigger;

            // Transform のスケールを取得
            Vector3 baseSize = thisScript.transform.localScale;

            // BoxCollider2D のサイズ計算
            Vector3 size = Vector3.Scale(baseSize, t.size); // 当たり判定のサイズ調整
            size.x += Mathf.Abs(t.x) * baseSize.x; // x方向に1サイズ分拡張
            size.y += Mathf.Abs(t.y) * baseSize.y; // y方向に1サイズ分拡張

            // BoxCollider2D のオフセット計算（isCenterがfalseの場合、拡張方向に1マス分ずらす）
            Vector3 offset = Vector3.zero;
            if (!t.isCenter)
            {
                if (t.x != 0) offset.x = (t.x / 2) * baseSize.x; // x方向に1マス分ずらす
                if (t.y != 0) offset.y = (t.y / 2) * baseSize.y; // y方向に1マス分ずらす
            }

            // 実際の位置
            Vector3 pos = thisScript.transform.position + offset;


            // 四角形の頂点を計算
            Vector3[] rectPoints =
            {
                pos + new Vector3(-size.x / 2, -size.y / 2, 0), // 左下
                pos + new Vector3( size.x / 2, -size.y / 2, 0), // 右下
                pos + new Vector3( size.x / 2,  size.y / 2, 0), // 右上
                pos + new Vector3(-size.x / 2,  size.y / 2, 0), // 左上
                pos + new Vector3(-size.x / 2, -size.y / 2, 0)  // 左下（閉じる）
            };

            // Sceneビューに枠線を描画
            Handles.color = Color.green;
            Handles.DrawPolyLine(rectPoints);
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


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Editor状でTrapをDebugで発動する専用のボタンです。");

        // StratEventTrap() を実行するボタンを作成
        if (GUILayout.Button("Start Event Trap"))
        {
            thisScript.StratEventTrap();
        }
    }
}
#endif