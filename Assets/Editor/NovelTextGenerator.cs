using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NovelTextGenerator : EditorWindow
{
    [MenuItem("Tools/NovelGenerator")]
    static void ShowWindow()
    {
        var Window=GetWindow<NovelTextGenerator>();

        Window.minSize= new Vector2(800, 500);
        Window.maxSize = new Vector2(800,500);
    }

    private string NovelContent = "";

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        //日本語入力に変更できるようにする
        Input.imeCompositionMode = IMECompositionMode.On;

        //文字入力できる領域を作る（改行可能）
        NovelContent = GUILayout.TextArea(NovelContent, GUILayout.Height(200));
    }
}
