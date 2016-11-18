using UnityEngine;
using UnityEditor;

public class LevelCreatorEditor : EditorWindow
{
    LevelCreator _target;

    [MenuItem("Window/Lagsters/Level Editor %#l")]
    static void Init()
    {
        EditorWindow.GetWindow<LevelCreatorEditor>("Level Editor");
    }

    void Awake()
    {
        _target = target as LevelCreator;
        // ReloadSerializedProperties();
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("LevelEditorPath"))
        {
            //string objectPath = EditorPrefs.GetString("ObjectPath");
            //pathPoints = AssetDatabase.LoadAssetAtPath(objectPath, typeof(PathPoints)) as PathPoints;
        }
    }

    private void OnSceneGUI()
    {
        if (_target.pathPoints.pointsList.Count <= 0)
            return;

        /*if (_target.pathPoints.activeIndex < 0)
            return;

        int startDraw = _target.activeIndex - drawPointsCount;
        int endDraw = _target.activeIndex + drawPointsCount;
        if (startDraw < 0)
        {
            endDraw = endDraw + Mathf.Abs(startDraw);
            startDraw = 0;
        }
        if (endDraw > _target.points.Count - 1)
            endDraw = _target.points.Count - 1;

        Transform handleTransform = _target.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        /*Handles.color = Color.blue;
        for (int i = startDraw; i <= endDraw; i++)
        {
            if (_target.points[i] != null)
            {
                Vector3 pointInWorldSpace = handleTransform.TransformPoint(_target.points[i].position);
                Quaternion rotation = _target.points[i].rotation;

                // Connect points with line
                if (i > startDraw)
                {
                    Vector3 prevPointInWorldSpace = handleTransform.TransformPoint(_target.points[i - 1].position);
                    Handles.DrawLine(pointInWorldSpace, prevPointInWorldSpace);
                }

                Handles.SphereCap(i, pointInWorldSpace, handleRotation, 0.5f);

                if (i == _target.activeIndex)
                {
                    EditorGUI.BeginChangeCheck();
                    pointInWorldSpace = Handles.DoPositionHandle(pointInWorldSpace, rotation);
                    if (EditorGUI.EndChangeCheck())
                        ChangePointCoordinates(handleTransform.InverseTransformPoint(pointInWorldSpace));

                    EditorGUI.BeginChangeCheck();
                    rotation = Handles.RotationHandle(rotation, pointInWorldSpace);
                    if (EditorGUI.EndChangeCheck())
                        ChangePointRotation(rotation);
                }
            }
        }*/
    } 
}
