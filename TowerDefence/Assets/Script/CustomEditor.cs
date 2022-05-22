using UnityEditor;
using UnityEngine;

public class CustomEditor : EditorWindow
{
    [MenuItem("Tools/CustomWindow")]
    public static void ShowWindow()
    {
        GetWindow<CustomEditor>("WaveEditor");
    }
    private void OnGUI()
    {
        GUILayout.Label("Wave Composition",EditorStyles.boldLabel);
        if (GUILayout.Button("Make a wave"))
        {
            GameObject.Find("GameMasta").GetComponent<LoadExcel>().LoadData();
        }
    }
}
