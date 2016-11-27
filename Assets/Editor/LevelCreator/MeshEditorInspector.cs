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
    SerializedProperty extrudeMesh;
    SerializedProperty generatedSplineStep;

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
        extrudeMesh       = serializedObject.FindProperty("extrudeMesh");
        generatedSplineStep = serializedObject.FindProperty("generatedSplineStep");
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
        EditorGUILayout.PropertyField(extrudeMesh, new GUIContent("Extrude Mesh"));

        GUI.enabled = false;
        EditorGUILayout.PropertyField(generatedSplineStep, new GUIContent("Generated Step"));
        GUI.enabled = true;

        EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
        EditorGUILayout.Slider(positionOnSpline, 0f, 1f, GUIContent.none);

        EditorGUILayout.LabelField("Step", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntSlider(currentSplineStep, 0, _target.splineSteps - 1, GUIContent.none);
        if (GUILayout.Button("<<"))
        {
            if(_target.currentSplineStep > 0)
                _target.currentSplineStep--;
            else
                _target.currentSplineStep = _target.splineSteps - 1;
            SceneView.RepaintAll();
        }
        if (GUILayout.Button(">>"))
        {
            if (_target.currentSplineStep < _target.splineSteps - 1)
                _target.currentSplineStep++;
            else
                _target.currentSplineStep = 0;
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();

        //GUI.enabled = ! _target.isMeshEmpty;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create First Step"))
        {
            _target.CreateFirstStepMesh();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Create Next Step"))
        {
            for (int i = 0; i < _target.splineSteps; i++)
            {
                if (i == 0)
                    _target.CreateFirstStepMesh();
                else if (i == _target.splineSteps - 1)
                    _target.CreateLastStepMesh();
                else
                    _target.CreateNextStepMesh();

                SceneView.RepaintAll();
            }
        }
        if (GUILayout.Button("Create Closed"))
        {
            _target.CreateMesh();
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
        if (GUILayout.Button("Generate Edge"))
        {
            _target.CreateNewEdge( _target.currentSplineStep );
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Extrude edge"))
        {
            _target.ExtrudeMesh();
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
