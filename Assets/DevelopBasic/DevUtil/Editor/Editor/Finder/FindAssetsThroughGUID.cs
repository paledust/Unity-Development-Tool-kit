using UnityEditor;
using UnityEngine;

public class FindAssetsThroughGUID : EditorWindow
{
    private string guid = "";

    [MenuItem("Tools/Others/Find Assets Through GUID")]
    public static void ShowWindow()
    {
        GetWindow<FindAssetsThroughGUID>("Find Assets Through GUID");
    }

    void OnGUI()
    {
        GUILayout.Label("Find Assets Through GUID", EditorStyles.boldLabel);

        guid = EditorGUILayout.TextField("GUID:", guid);

        if (GUILayout.Button("Find Assets"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
    }
}
