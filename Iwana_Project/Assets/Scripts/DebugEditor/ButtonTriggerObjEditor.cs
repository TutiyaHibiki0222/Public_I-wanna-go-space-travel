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