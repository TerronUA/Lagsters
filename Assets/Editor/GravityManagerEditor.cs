using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GravityManager))]
public class GravityManagerEditor : Editor
{
    GravityManager _target;
    SerializedProperty activeItemIndex;
    SerializedProperty activeItem;
    SerializedProperty offset;
    SerializedProperty useLastPointGravity;
    SerializedProperty gravity;
    SerializedProperty pathPoints;
    SerializedProperty layerMask;
    static GravityManagerEditor editor;

    const int drawPointsCount = 3;



    /// <summary>
    /// Update all serialized properties
    /// </summary>
    void ReloadSerializedProperties()
    {
        activeItemIndex = serializedObject.FindProperty("activeIndex");
        activeItem = serializedObject.FindProperty("activePoint");
        offset = serializedObject.FindProperty("offset");
        useLastPointGravity = serializedObject.FindProperty("useLastPointGravity");
        gravity = serializedObject.FindProperty("gravity");
        pathPoints = serializedObject.FindProperty("pathPoints");
        layerMask = serializedObject.FindProperty("layerMask");
    }

    /// <summary>
    /// Change active point coordinates
    /// </summary>
    /// <param name="newVector">New coordinates</param>
    void ChangePointCoordinates(Vector3 newVector)
    {
        serializedObject.Update();

        SerializedProperty activeItemPos = activeItem.FindPropertyRelative("position");

        activeItemPos.FindPropertyRelative("x").floatValue = newVector.x;
        activeItemPos.FindPropertyRelative("y").floatValue = newVector.y;
        activeItemPos.FindPropertyRelative("z").floatValue = newVector.z;

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Change active point rotation
    /// </summary>
    /// <param name="rotation">New rotation</param>
    void ChangePointRotation(Quaternion rotation)
    {
        serializedObject.Update();

        SerializedProperty activeItemRotation = activeItem.FindPropertyRelative("rotation");
        activeItemRotation.quaternionValue = rotation;

        serializedObject.ApplyModifiedProperties();
    }

    void Awake()
    {
        _target = target as GravityManager;
        ReloadSerializedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ReloadSerializedProperties();

        //base.OnInspectorGUI();

        DrawDefaultParms();

        DrawSelectedPoint();

        // Save changed properties
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (_target.points.Count <= 0)
            return;

        if (_target.activeIndex < 0)
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

        Handles.color = Color.blue;
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
        }
    }

    /// <summary>
    /// Draw defaule point params and actions buttons
    /// </summary>
    private void DrawDefaultParms()
    {
        GUILayout.Label("Default params", EditorStyles.boldLabel);

        EditorGUILayout.Slider(offset, 0, 100, new GUIContent("Offset"));

        // Controls for gravity
        Rect position = EditorGUILayout.GetControlRect();

        GUIContent label = EditorGUI.BeginProperty(position, new GUIContent("Last Point Gravity", "Copy gravity value from the last point"), useLastPointGravity);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        contentPosition.width *= 0.25f;
        useLastPointGravity.boolValue = EditorGUI.Toggle(contentPosition, useLastPointGravity.boolValue);

        // Gravity field enabled only if LastPointGravity is unchecked
        GUI.enabled = !useLastPointGravity.boolValue;

        contentPosition.x += contentPosition.width * 2;
        EditorGUI.LabelField(contentPosition, "Gravity");

        contentPosition.x += contentPosition.width;
        EditorGUI.PropertyField(contentPosition, gravity, GUIContent.none);

        GUI.enabled = true;

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        // Actions buttons
        GUILayout.Label("Actions", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add"))
        {
            _target.activeIndex = _target.AddPoint();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Delete"))
        {
            _target.DeletePoint();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Delete All"))
        {
            if (EditorUtility.DisplayDialog("Delete all points?", "You really want to delete all points?", "Yes", "No"))
            {
                _target.DeleteAllPoints();
                SceneView.RepaintAll();
            }
        }

        GUILayout.EndHorizontal();
    }

    private void DrawSelectedPoint()
    {
        EditorGUILayout.LabelField("Selected Item", EditorStyles.boldLabel);
        EditorGUILayout.IntSlider(activeItemIndex, 0, _target.points.Count - 1, GUIContent.none);

        // Display selected item
        _target.UpdateActivePoint();

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        float labelWidth = EditorGUIUtility.labelWidth;
        try
        {
            // Display point position
            Rect position = EditorGUILayout.GetControlRect();

            GUIContent label = EditorGUI.BeginProperty(position, new GUIContent("Position"), activeItem);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            // Show point position and gravity
            EditorGUIUtility.labelWidth = 14f;
            contentPosition.width *= 0.333f;
            SerializedProperty activeItemPos = activeItem.FindPropertyRelative("position");

            EditorGUI.PropertyField(contentPosition, activeItemPos.FindPropertyRelative("x"));

            contentPosition.x += contentPosition.width;
            EditorGUI.PropertyField(contentPosition, activeItemPos.FindPropertyRelative("y"));

            contentPosition.x += contentPosition.width;
            EditorGUI.PropertyField(contentPosition, activeItemPos.FindPropertyRelative("z"));

            EditorGUI.EndProperty();

            // Display point rotation
            SerializedProperty activeItemRotation = activeItem.FindPropertyRelative("rotation");
            position = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = labelWidth;
            label = EditorGUI.BeginProperty(position, new GUIContent("Rotation"), activeItemRotation);

            contentPosition = EditorGUI.PrefixLabel(position, label);
            EditorGUIUtility.labelWidth = 14f;
            //contentPosition.width *= 0.333f;

            Vector3 eulerRotation = activeItemRotation.quaternionValue.eulerAngles;

            eulerRotation = EditorGUI.Vector3Field(contentPosition, GUIContent.none, eulerRotation);
            activeItemRotation.quaternionValue = Quaternion.Euler(eulerRotation);

            //EditorGUI.PropertyField(contentPosition, activeItemRotation.FindPropertyRelative("x"));

            //contentPosition.x += contentPosition.width;
            //EditorGUI.PropertyField(contentPosition, activeItemRotation.FindPropertyRelative("y"));

            //contentPosition.x += contentPosition.width;
            //EditorGUI.PropertyField(contentPosition, activeItemRotation.FindPropertyRelative("z"));

            EditorGUI.EndProperty();

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.Slider(activeItem.FindPropertyRelative("gravity"), -20, 20, new GUIContent("Gravity"));

            EditorGUILayout.PropertyField(layerMask, new GUIContent("Layer"));

            GUILayout.BeginHorizontal();

            GUI.enabled = _target.points.Count > 1;
            if (GUILayout.Button("Center in layer"))
            {
                _target.RealignPoint();
                SceneView.RepaintAll();
            }
            GUI.enabled = true;
            if (GUILayout.Button("Find in scene"))
            {
                SceneView.lastActiveSceneView.pivot = activeItemPos.vector3Value;
                SceneView.lastActiveSceneView.Repaint();
            }
            GUILayout.EndHorizontal();
        }
        finally
        {
            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}
