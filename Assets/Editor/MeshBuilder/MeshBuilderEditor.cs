using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelSpline;

[CustomEditor(typeof(MeshBuilder))]
public class MeshBuilderEditor : Editor
{
    private MeshBuilder builder;
    private GUIContent btnCreateFirstEdge;
    private GUIContent btnAppendToMesh;

    void OnEnable()
    {
        if (btnCreateFirstEdge == null)
            btnCreateFirstEdge = new GUIContent((Texture)Resources.Load("btnCreateFirstEdge"), "Create first edge for mesh");
        if (btnAppendToMesh == null)
            btnAppendToMesh = new GUIContent((Texture)Resources.Load("btnAppendToMesh"), "Append edge to Mesh");
    }

    public override void OnInspectorGUI()
    {
        builder = target as MeshBuilder;

        MeshBuilderGUI();
        SplineSelectionGUI();
        MeshGenerationGUI();
    }

    private void MeshBuilderGUI()
    {
        EditorGUI.BeginChangeCheck();
        Mesh newMesh = (Mesh)EditorGUILayout.ObjectField("Mesh", builder.mesh, typeof(Mesh), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(builder, "Mesh Data");
            EditorUtility.SetDirty(builder);
            builder.mesh = newMesh;
        }

        EditorGUI.BeginChangeCheck();
        BezierSplineData newData = (BezierSplineData)EditorGUILayout.ObjectField("Spline Data", builder.spline, typeof(BezierSplineData), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(builder, "Spline Data");
            EditorUtility.SetDirty(builder);
            builder.spline = newData;
        }

        EditorGUI.BeginChangeCheck();
        int pointsOnCircle = EditorGUILayout.IntField("Points on circle", builder.pointsOnCircle);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(builder, "Change Points on Circle");
            EditorUtility.SetDirty(builder);
            builder.pointsOnCircle = pointsOnCircle;
        }

        EditorGUI.BeginChangeCheck();
        int splineSteps = EditorGUILayout.IntField("Spline steps", builder.splineSteps);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(builder, "Change spline steps amount");
            EditorUtility.SetDirty(builder);
            builder.splineSteps = splineSteps;
        }

        EditorGUI.BeginChangeCheck();
        float radius = EditorGUILayout.FloatField("Radius", builder.radius);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(builder, "Change radius");
            EditorUtility.SetDirty(builder);
            builder.radius = radius;
        }
    }

    private void SplineSelectionGUI()
    {
        GUIStyle styleLabel = new GUIStyle(GUI.skin.label);
        styleLabel.fontSize = GUI.skin.font.fontSize + 2;
        styleLabel.fontStyle = FontStyle.Bold;
        styleLabel.clipping = TextClipping.Overflow;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Spline selection", styleLabel);

        EditorGUI.BeginChangeCheck();
        int selectedIndex = EditorGUILayout.IntSlider("Selected point", builder.selectedIndex, 0, builder.PointsOnSpline - 1);
        if (EditorGUI.EndChangeCheck())
        {
            builder.SelectedPoint = selectedIndex;
            SceneView.RepaintAll();
        }

        EditorGUI.BeginChangeCheck();
        int selectedEdge = EditorGUILayout.IntSlider("Selected edge", builder.selectedEdge, 0, builder.EdgesFromSelectedPoint - 1);
        if (EditorGUI.EndChangeCheck())
        {
            builder.selectedEdge = selectedEdge;
            SceneView.RepaintAll();
        }

        EditorGUI.BeginChangeCheck();
        float positionOnEdge = EditorGUILayout.Slider("Position", builder.positionOnEdge, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            builder.PositionOnEdge = positionOnEdge;
            SceneView.RepaintAll();
        }
    }

    private void MeshGenerationGUI()
    {
        GUIStyle styleLabel = new GUIStyle(GUI.skin.label);
        styleLabel.fontSize = GUI.skin.font.fontSize + 2;
        styleLabel.fontStyle = FontStyle.Bold;
        styleLabel.clipping = TextClipping.Overflow;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Mesh generation", styleLabel);

        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.fixedWidth = 40f;
        styleButton.fixedHeight = 40f;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(btnCreateFirstEdge, styleButton))
        {
            Undo.RecordObject(builder.mesh, "Generate first step mesh");
            EditorUtility.SetDirty(builder.mesh);
            builder.CreateFirstStepMesh();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button(btnAppendToMesh, styleButton))
        {
            Undo.RecordObject(builder.mesh, "Generate next step mesh");
            EditorUtility.SetDirty(builder.mesh);
            builder.CreateNextStepMesh();
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        builder = target as MeshBuilder;
        DrawSelectedEdge();
    }

    public void DrawSelectedEdge()
    {
        Vector3 startPosition;
        Vector3 endPosition;
        Vector3 startRightPoint;
        Vector3 endLeftPoint;
        Color drawColor;

        if (builder.GetSelectedEdgePoints(out startPosition, out endPosition, out startRightPoint, out endLeftPoint, out drawColor))
        {
            Handles.DrawBezier(startPosition, endPosition, startRightPoint, endLeftPoint, drawColor, null, 2f);

            Handles.color = Color.red;
            Handles.SphereCap(-1, startRightPoint, Quaternion.identity, 3f);
            Handles.SphereCap(-1, endLeftPoint, Quaternion.identity, 3f);
        }
    }
}
