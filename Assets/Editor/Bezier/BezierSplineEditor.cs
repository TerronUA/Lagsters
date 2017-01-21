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
        private int indexTangentStart = -1;
        private GUIContent btnAddAfter;
        private GUIContent btnAddBefore;
        private GUIContent btnAddBranch;
        private GUIContent btnAddEdge;
        private GUIContent btnDeletePoint;

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
            if (btnDeletePoint == null)
                btnDeletePoint = new GUIContent((Texture)Resources.Load("bttnSplineDeletePoint"), "Delete selected point");
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
            else
            {
                if (GUILayout.Button("Repair spline"))
                {
                    Undo.RecordObject(spline.splineData, "Repair spline");
                    EditorUtility.SetDirty(spline.splineData);
                    spline.RepairData();
                    selectedIndex = -1;
                    selectedCPointIndex = -1;
                }
            }

            if (0 <= selectedIndex && selectedIndex < spline.PointsCount)
            {
                DrawSelectedPointInspector();
            }
        }

        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point: " + selectedIndex.ToString());

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
                Undo.RecordObject(spline.splineData, "Add point");
                EditorUtility.SetDirty(spline.splineData);
                selectedIndex = spline.AddBranchAfter(selectedIndex);
            }

            if (GUILayout.Button(btnAddEdge))
            {
                indexNewEdgeStart = selectedIndex;
                Repaint();
            }

            if (GUILayout.Button(btnDeletePoint))
            {
                Undo.RecordObject(spline.splineData, "Delete Point");
                EditorUtility.SetDirty(spline.splineData);
                spline.DeletePoint(selectedIndex);
                selectedIndex = -1;
                selectedCPointIndex = -1;
            }
            GUILayout.EndHorizontal();

            if (indexNewEdgeStart >= 0)
            {
                EditorGUILayout.HelpBox("Select final point for the edge", MessageType.Warning);
            }

            GUI.enabled = selectedIndex >= 0 && selectedCPointIndex >= 0;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Enforce tangent"))
            {
                indexTangentStart = selectedCPointIndex;
                /*
                Undo.RecordObject(spline.splineData, "Enforce tangent");
                EditorUtility.SetDirty(spline.splineData);
                spline.EnforceTangent(selectedIndex);
                */
            }
            if (GUILayout.Button("Revert edge"))
            {
                Undo.RecordObject(spline.splineData, "Revert edge");
                EditorUtility.SetDirty(spline.splineData);
                spline.RevertEdge(selectedIndex, selectedCPointIndex);
            }
            if (GUILayout.Button("Delete edge"))
            {
                Undo.RecordObject(spline.splineData, "Delete edge");
                EditorUtility.SetDirty(spline.splineData);
                spline.DeleteEdge(selectedIndex, selectedCPointIndex);
            }
            GUILayout.EndHorizontal();
            if (indexTangentStart >= 0)
            {
                EditorGUILayout.HelpBox("Select control point to enforce tangent", MessageType.Warning);
            }
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

            Vector3 pointPosition = handleTransform.TransformPoint(pt.position);
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
                    if (indexNewEdgeStart < 0 && (indexTangentStart < 0 || selectedIndex != index))
                    {
                        selectedIndex = index;
                        selectedCPointIndex = i;
                        indexTangentStart = -1;
                    }
                    else if (indexNewEdgeStart >= 0)
                    {
                        Undo.RecordObject(spline.splineData, "Add edge");
                        EditorUtility.SetDirty(spline.splineData);
                        spline.AddEdgeBetween(selectedIndex, index);
                        indexNewEdgeStart = -1;
                        indexTangentStart = -1;
                    }
                    else if (indexTangentStart >= 0 && i >= 0)
                    {
                        Undo.RecordObject(spline.splineData, "Enforce tangent");
                        EditorUtility.SetDirty(spline.splineData);
                        spline.EnforceTangent(selectedIndex, indexTangentStart, i);
                        indexNewEdgeStart = -1;
                        indexTangentStart = -1;
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

            Vector3 startPosition = handleTransform.TransformPoint(pt.position);
            Vector3 endPosition;
            Vector3 startRightPoint;
            Vector3 endLeftPoint;

            foreach (int nextPointIndex in nextPoints)
            {
                nextPoint = spline.GetPoint(nextPointIndex);
                endPosition = handleTransform.TransformPoint(nextPoint.position);

                startRightPoint = handleTransform.TransformPoint(pt.GetNextPointPositionTo(nextPointIndex));
                endLeftPoint = handleTransform.TransformPoint(nextPoint.GetPrevPointPositionTo(index));

                Handles.DrawBezier(startPosition, endPosition, startRightPoint, endLeftPoint, Color.white, null, 2f);
            }
        }
    }
}