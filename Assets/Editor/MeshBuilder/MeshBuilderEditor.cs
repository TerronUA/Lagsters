using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelSpline;

[CustomEditor(typeof(MeshBuilder))]
public class MeshBuilderEditor : Editor
{
    private MeshBuilder builder;

    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        builder = target as MeshBuilder;

        MeshBuilderGUI();
    }

    private void MeshBuilderGUI()
    {
        EditorGUI.BeginChangeCheck();
        LevelSpline.BezierSpline spline = (LevelSpline.BezierSpline)EditorGUILayout.ObjectField("Spline", builder.spline, typeof(LevelSpline.BezierSpline), true);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(builder, "Spline changes");
            EditorUtility.SetDirty(builder);
            builder.spline = spline;
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
        /*
    public int currentSplineStep = 0;
    public int generatedSplineStep = 0;
         */
    }

    private void OnSceneGUI()
    {
    }
}
