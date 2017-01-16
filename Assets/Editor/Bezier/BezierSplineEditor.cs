using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelSpline
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineEditor : Editor
    {
        private const int stepsPerCurve = 10;
        private const float directionScale = 0.5f;
        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;

        private BezierSpline spline;
        private Transform handleTransform;
        private Quaternion handleRotation;
        private int selectedIndex = -1;
        private int selectedCPointIndex = -1;

        /// <summary>
        /// Create new scriptable object for Bezier Level Spline
        /// </summary>
        [MenuItem("Lagsters/Create Level Spline")]
        static void CreateLevelSpline()
        {
            string path = EditorUtility.SaveFilePanel("Create Level Spline Data", "Assets/ScriptableObjects/", "new.asset", "asset");
            if (path == "")
                return;

            path = FileUtil.GetProjectRelativePath(path);

            BezierSplineData spline = CreateInstance<BezierSplineData>();

            AssetDatabase.CreateAsset(spline, path);
            AssetDatabase.SaveAssets();
        }

        
        public override void OnInspectorGUI()
        {
            //if (Event.current.type == EventType.Layout)
            //    return;

            spline = target as BezierSpline;
            EditorGUI.BeginChangeCheck();
            BezierSplineData newData = (BezierSplineData)EditorGUILayout.ObjectField("Spline Data", spline.splineData, typeof(BezierSplineData), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Spline Data");
                EditorUtility.SetDirty(spline);
                spline.splineData = newData;
            }
            
            if (0 <= selectedIndex && selectedIndex < spline.PointsCount)
            {
                DrawSelectedPointInspector();
            }
        }

        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");
                        
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetPosition(selectedIndex, selectedCPointIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline.splineData, "Move Point");
                EditorUtility.SetDirty(spline.splineData);
                spline.SetPosition(selectedIndex, selectedCPointIndex, point);
            }

            GUI.enabled = selectedIndex >= 0 && selectedCPointIndex >= 0;
            if (GUILayout.Button("Add After"))
            {
                Undo.RecordObject(spline.splineData, "Add Point");
                EditorUtility.SetDirty(spline.splineData);
                spline.AddPointAfter(selectedIndex);
            }
        }

        private void OnSceneGUI()
        {
            spline = target as BezierSpline;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            //BezierPoint p0 = spline.GetPoint(0);
            //Vector3 p0 = ShowPoint(0);
            for (int i = 0; i < spline.PointsCount; i++)
            {
                ShowPoint(i);
                DrawEdges(i);
            }
            //ShowDirections();
        }

        /// <summary>
        /// Draws BezierPoint and it's control points
        /// </summary>
        /// <param name="pt">Bezier Point</param>
        /// <param name="index">Index of pt in spline</param>
        /// <returns></returns>
        private Vector3 ShowPoint(int index)
        {
            BezierPoint pt = spline.GetPoint(index);

            Vector3 pointPosition = handleTransform.TransformPoint(pt.Point);
            for (int i = 0; i < pt.points.Length; i++)
            {
                Vector3 point = handleTransform.TransformPoint(pt.points[i]);

                float size = HandleUtility.GetHandleSize(point);
                if (i == 0)
                {
                    size *= 2f;
                    Handles.color = Color.white;
                }
                else if (pt.IsLeftPointIndex(i))
                    Handles.color = Color.red;
                else
                    Handles.color = Color.green;

                if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
                {
                    selectedIndex = index;
                    selectedCPointIndex = i;
                    Repaint();
                }

                if ((selectedIndex == index) && (selectedCPointIndex == i))
                {
                    EditorGUI.BeginChangeCheck();
                    point = Handles.DoPositionHandle(point, handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(spline.splineData, "Move Point");
                        EditorUtility.SetDirty(spline.splineData);
                        pt.SetPosition(i, handleTransform.InverseTransformPoint(point));
                    }
                }

                // Draw lines between point and control points, and dont draw line to itself
                if (i != 0)
                {
                    Handles.color = Color.gray;
                    Handles.DrawLine(pointPosition, point);
                }
            }

            return pointPosition;
        }

        /// <summary>
        /// Draw all adges to nex point for Bezier point
        /// </summary>
        /// <param name="index">Index of the Bezier point in spline</param>
        public void DrawEdges(int index)
        {
            BezierPoint pt = spline.GetPoint(index);
            List<BezierPoint> nextPoints = spline.GetNextPoints(index);
            int i = 0;

            Vector3 startPosition = handleTransform.TransformPoint(pt.Point);
            Vector3 endPosition;
            Vector3 startRightPoint;
            Vector3 endLeftPoint;

            foreach (BezierPoint nextPoint in nextPoints)
            {
                endPosition = handleTransform.TransformPoint(nextPoint.Point);

                startRightPoint = handleTransform.TransformPoint(pt.GetRightPoint(i));
                endLeftPoint = handleTransform.TransformPoint(nextPoint.GetLeftPoint(0));

                Handles.DrawBezier(startPosition, endPosition, startRightPoint, endLeftPoint, Color.white, null, 2f);

                i++;
            }
        }
    }
}