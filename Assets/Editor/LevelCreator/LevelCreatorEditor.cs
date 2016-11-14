using UnityEngine;
using UnityEditor;

public class LevelCreatorEditor : EditorWindow
{
    [MenuItem("Window/Lagsters/Level Editor %#l")]
    static void Init()
    {
        EditorWindow.GetWindow<LevelCreatorEditor>("Level Editor");
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("LevelEditorPath"))
        {
            //string objectPath = EditorPrefs.GetString("ObjectPath");
            //pathPoints = AssetDatabase.LoadAssetAtPath(objectPath, typeof(PathPoints)) as PathPoints;
        }
    }
}
