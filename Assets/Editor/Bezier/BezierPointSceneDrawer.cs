using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelSpline
{
    public static class BezierPointSceneDrawer
    {
#if UNITY_EDITOR

        public static void DrawPoint(this BezierPoint pt, Transform handleTransform)
        {
            for (int i = 0; i < pt.points.Length; i++)
            {
                Vector3 pointPosition = handleTransform.TransformPoint(pt.points[i]);
                float size = HandleUtility.GetHandleSize(pointPosition);
                if (i == 0)
                    size *= 2f;

                Handles.color = Color.white;
/*
                if (Handles.Button(pointPosition, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
                {
                    selectedIndex = index;
                    selectedCPointIndex = indexNextPoint;
                    Repaint();
                }

                if ((selectedIndex == index) && (selectedCPointIndex == indexNextPoint))
                {
                    EditorGUI.BeginChangeCheck();
                    pointPosition = Handles.DoPositionHandle(pointPosition, handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(spline, "Move Point");
                        EditorUtility.SetDirty(spline);
                        //spline.SetPointPosition(index, handleTransform.InverseTransformPoint(pointPosition));
                    }
                }
*/
            }

        }
#endif
    }
}
