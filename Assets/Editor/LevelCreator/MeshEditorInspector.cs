using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MeshEditor))]
public class MeshEditorInspector : Editor
{
    MeshEditor _target;
    SerializedProperty positionOnSpline;
    SerializedProperty currentSplineStep;
    SerializedProperty pointsOnCircle;
    SerializedProperty splineSteps;
    SerializedProperty radius;

    /// <summary>
    /// Update all serialized properties
    /// </summary>
    void ReloadSerializedProperties()
    {
        positionOnSpline  = serializedObject.FindProperty("positionOnSpline");
        currentSplineStep = serializedObject.FindProperty("currentSplineStep");
        pointsOnCircle    = serializedObject.FindProperty("pointsOnCircle");
        splineSteps       = serializedObject.FindProperty("splineSteps");
        radius            = serializedObject.FindProperty("radius");
    }
    
    void Awake()
    {
        _target = target as MeshEditor;
        _target.LoadMesh();
        ReloadSerializedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ReloadSerializedProperties();

        //base.OnInspectorGUI();
        EditorGUILayout.PropertyField(pointsOnCircle, new GUIContent("Points On Circle"));
        EditorGUILayout.PropertyField(splineSteps, new GUIContent("Spline Steps"));
        EditorGUILayout.PropertyField(radius, new GUIContent("Radius"));

        EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
        EditorGUILayout.Slider(positionOnSpline, 0f, 1f, GUIContent.none);

        EditorGUILayout.LabelField("Step", EditorStyles.boldLabel);
        EditorGUILayout.IntSlider(currentSplineStep, 0, _target.splineSteps - 1, GUIContent.none);

        if (GUILayout.Button("Generate Edge"))
        {
            _target.CreateNewEdge( 1f / _target.splineSteps * _target.currentSplineStep );
            SceneView.RepaintAll();
        }

        // Save changed properties
        serializedObject.ApplyModifiedProperties();
    }



    private void OnSceneGUI()
    {
        //Vector3 position = _target.spline.GetPoint(_target.positionOnSpline);
        Vector3 position = _target.spline.GetPoint(1f / _target.splineSteps * _target.currentSplineStep);
        Transform handleTransform = _target.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Handles.color = Color.blue;

        Vector3 pointInWorldSpace = handleTransform.TransformPoint(position);

        Handles.SphereCap(1, pointInWorldSpace, handleRotation, 0.5f);
    }
}
