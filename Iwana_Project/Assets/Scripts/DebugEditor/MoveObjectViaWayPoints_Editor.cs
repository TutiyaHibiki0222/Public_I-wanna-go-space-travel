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
        // ラベルのカスタムスタイルを作成
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white; // 文字色を白に設定
        // 背景を追加して文字を見やすくする
        labelStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.55f)); // 背景色を半透明の黒に設定


        for (int i = 0; i < thisScript.moves.Count; i++)
        {
            Vector3 point = thisScript.moves[i].position;

            if (point != null)  // `null` チェックは不要。`point` は `Vector3` 型なので
            {
                // ポイントの移動
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.PositionHandle(point, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    // Undo履歴を記録
                    Undo.RecordObject(thisScript, "Move Patrol Point");
                    thisScript.moves[i].position = newPosition;  // moves[i] の position を更新
                }

                // 円を描画
                Handles.DrawWireDisc(point, Vector3.forward, 0.01f);
            }
            // 矢印を描画
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

            // ラベル
            Handles.Label(point, $"Move Point: {i}", labelStyle);
        }
    }
    // お互いの方向を描画するメソッド.
    public void DrawArrowToArrow(Vector3 start, Vector3 end, Color color)
    {
        Handles.color = color;

        // 矢印の方向と長さ
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // 矢印の終点を手前にずらす
        float offset = 0.25f;
        Vector3 adjustedStart = end - direction * (distance - offset);
        Vector3 adjustedEnd = start + direction * (distance - offset);

        float lineWidth = 10.0f; // 線の太さ
        Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    adjustedStart - direction * (offset * 0.75f),
                    adjustedEnd + direction * (offset * 0.75f)
                });
        // 線の描画
        Handles.DrawLine(adjustedStart, adjustedEnd);
        // 矢印の描画
        float arrowSize = 0.2f; // 矢印のサイズ
        Handles.ArrowHandleCap(
            0,
            adjustedEnd, // ギリギリ手前の位置
            Quaternion.LookRotation( direction), // 矢印の向き
            arrowSize,
            EventType.Repaint
        );
        Handles.ArrowHandleCap(
            0,
            adjustedStart, // ギリギリ手前の位置
            Quaternion.LookRotation(-direction), // 矢印の向き
            arrowSize,
            EventType.Repaint
        );
    }
    // 矢印を描画するメソッド
    private void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        Handles.color = color;

        // 矢印の方向と長さ
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // 矢印の終点を手前にずらす
        float offset = 0.25f; // 終了点手前のオフセット量
        Vector3 adjustedEnd = start + direction * (distance - offset);
        
        float lineWidth = 10.0f; // 線の太さ
        Handles.DrawAAPolyLine(lineWidth, new Vector3[] {
                    start,
                    adjustedEnd + direction * (offset * 0.75f)
                });

        // 線の描画
        Handles.DrawLine(start, adjustedEnd);

        // 矢印の描画
        float arrowSize = 0.2f; // 矢印のサイズ
        Handles.ArrowHandleCap(
            0,
            adjustedEnd, // ギリギリ手前の位置
            Quaternion.LookRotation(direction), // 矢印の向き
            arrowSize,
            EventType.Repaint
        );
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
        machine = (Machine)target;
        EditorGUILayout.LabelField("MoveObjectViaWayPoints 用カスタム設定", EditorStyles.boldLabel);
        
        // ドラッグアンドドロップ受付用の領域を作成
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "ここに GameObject をドラッグ＆ドロップ", EditorStyles.helpBox);

        // ドラッグ処理
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
