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
        private int indexNewEdgeStart = -1;
        private int selectedCPointIndex = -1;
        private GUIContent btnAddAfter;
        private GUIContent btnAddBefore;
        private GUIContent btnAddBranch;
        private GUIContent btnAddEdge;

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

        void OnEnable()
        {
            if (btnAddAfter == null)
                btnAddAfter = new GUIContent((Texture)Resources.Load("bttnSplineAddAfter"), "Add point After selected point");
            if (btnAddBefore == null)
                btnAddBefore = new GUIContent((Texture)Resources.Load("bttnSplineAddBefore"), "Add point Before selected point");
            if (btnAddBranch == null)
                btnAddBranch = new GUIContent((Texture)Resources.Load("bttnSplineAddBranch"), "Add branch in selected point");
            if (btnAddEdge == null)
                btnAddEdge = new GUIContent((Texture)Resources.Load("bttnSplineAddEdge"), "Add edge to other point.\nClick on other point to define end");
        }

        public override void OnInspectorGUI()
        {
            //if (Event.current.type == EventType.Layout)
            //    return;
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                Repaint();
                return;
            }

            spline = target as BezierSpline;
            EditorGUI.BeginChangeCheck();
            BezierSplineData newData = (BezierSplineData)EditorGUILayout.ObjectField("Spline Data", spline.splineData, typeof(BezierSplineData), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Spline Data");
                EditorUtility.SetDirty(spline);
                spline.splineData = newData;
            }

            if (spline.PointsCount == 0)
            {
                if (GUILayout.Button("Add First Point"))
                {
                    Undo.RecordObject(spline.splineData, "Add Point");
                    EditorUtility.SetDirty(spline.splineData);
                    spline.AddFirstPoint();
                }
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

            GUI.enabled = selectedIndex >= 0 && selectedCPointIndex == -1;
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(btnAddBefore))
            {
                Undo.RecordObject(spline.splineData, "Add Point");
                EditorUtility.SetDirty(spline.splineData);
                selectedIndex = spline.AddPointBefore(selectedIndex);
            }

            if (GUILayout.Button(btnAddAfter))
            {
                Undo.RecordObject(spline.splineData, "Add Point");
                EditorUtility.SetDirty(spline.splineData);
                selectedIndex = spline.AddPointAfter(selectedIndex);
            }
            
            if (GUILayout.Button(btnAddBranch))
            {
                Undo.RecordObject(spline.splineData, "Add Point");
                EditorUtility.SetDirty(spline.splineData);
                selectedIndex = spline.AddBranchAfter(selectedIndex);
            }

            if (GUILayout.Button(btnAddEdge))
            {
                indexNewEdgeStart = selectedIndex;
            }
            GUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            spline = target as BezierSpline;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

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
        /// <param name="index">Index of pt in spline</param>
        /// <returns></returns>
        private void ShowPoint(int index)
        {
            BezierPoint pt = spline.GetPoint(index);

            Vector3 pointPosition = handleTransform.TransformPoint(pt.Point);
            for (int i = -1; i < pt.points.Length; i++)
            {
                Vector3 point = handleTransform.TransformPoint(pt.GetPosition(i));

                float size = HandleUtility.GetHandleSize(point);
                if (i < 0)
                {
                    size *= 2f;
                    Handles.color = Color.white;
                }
                else if (pt.IsPrevCPointIndex(i))
                    Handles.color = Color.red;
                else
                    Handles.color = Color.green;

                if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
                {
                    if (indexNewEdgeStart < 0)
                    {
                        selectedIndex = index;
                        selectedCPointIndex = i;
                    }
                    else
                    {
                        spline.AddEdgeBetween(selectedIndex, index);
                        indexNewEdgeStart = -1;
                    }
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
                        Repaint();
                    }
                }

                // Draw lines between point and control points, and dont draw line to itself
                if (i >= 0)
                {
                    Handles.color = Color.gray;
                    Handles.DrawLine(pointPosition, point);
                }
            }
        }

        /// <summary>
        /// Draw all adges to nex point for Bezier point
        /// </summary>
        /// <param name="index">Index of the Bezier point in spline</param>
        public void DrawEdges(int index)
        {
            BezierPoint pt = spline.GetPoint(index);
            BezierPoint nextPoint;
            List<int> nextPoints = spline.GetNextPointsIndexes(index);

            Vector3 startPosition = handleTransform.TransformPoint(pt.Point);
            Vector3 endPosition;
            Vector3 startRightPoint;
            Vector3 endLeftPoint;

            foreach (int nextPointIndex in nextPoints)
            {
                nextPoint = spline.GetPoint(nextPointIndex);
                endPosition = handleTransform.TransformPoint(nextPoint.position);

                startRightPoint = handleTransform.TransformPoint(pt.GetNextPointTo(nextPointIndex));
                endLeftPoint = handleTransform.TransformPoint(nextPoint.GetPrevPointTo(index));

                Handles.DrawBezier(startPosition, endPosition, startRightPoint, endLeftPoint, Color.white, null, 2f);
            }
        }
    }
}