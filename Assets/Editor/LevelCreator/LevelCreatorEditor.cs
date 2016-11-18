using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class LevelCreatorEditor : EditorWindow
{
    public LevelSpline splinePoints;

    private string openedSplineAssetName = "";

    private int viewIndex = 0;

    [MenuItem("Window/Lagsters/Level Spline Editor %#L")]
    static void Init()
    {
        EditorWindow.GetWindow<LevelCreatorEditor>("Level Editor");
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("LevelSplinePath"))
        {
            string objectPath = EditorPrefs.GetString("LevelSplinePath");
            splinePoints = AssetDatabase.LoadAssetAtPath(objectPath, typeof(LevelSpline)) as LevelSpline;
            openedSplineAssetName = objectPath;
        }
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void CreateNewSpline()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        string path = EditorUtility.SaveFilePanel("Select file for new spline", "", "LevelSpline", "asset");
        if (path.Length != 0)
        {
            string relPath = path.Substring(Application.dataPath.Length - "Assets".Length);
            viewIndex = -1;
            splinePoints = LevelSplineCreator.Create(relPath);
            if (splinePoints)
            {
                splinePoints.pointsList = new List<LevelSplinePoint>();
                relPath = AssetDatabase.GetAssetPath(splinePoints);
                EditorPrefs.SetString("LevelSplinePath", relPath);
                openedSplineAssetName = relPath;
            }
        }
    }


    void OpenItemList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Spline List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            splinePoints = AssetDatabase.LoadAssetAtPath(relPath, typeof(LevelSpline)) as LevelSpline;
            if (splinePoints.pointsList == null)
                splinePoints.pointsList = new List<LevelSplinePoint>();
            if (splinePoints)
            {
                EditorPrefs.SetString("LevelSplinePath", relPath);
                openedSplineAssetName = relPath;
            }
        }
    }

    void AddItem()
    {
        LevelSplinePoint newItem = new LevelSplinePoint();
        newItem.points = new List<Vector3>();

        int prevI = splinePoints.pointsList.Count - 1;
        int prevPrevI = splinePoints.pointsList.Count - 2;

        if (prevI >= 0)
        {
            if (splinePoints.pointsList[prevI].points.Count > 0)
                newItem.points.Add(splinePoints.pointsList[prevI].points[0]);
            if ((prevPrevI > 0) && (splinePoints.pointsList[prevPrevI].points.Count > 0))
            {
                Vector3 dir = splinePoints.pointsList[prevI].points[0] - splinePoints.pointsList[prevPrevI].points[0];
                newItem.points[0] += splinePoints.distance * dir.normalized;
            }
        }
        else
            newItem.points.Add(Vector3.zero);

        /*
        if (splinePoints.pointsList.Count >= 2)
        {
            Vector3 dir = 
        }
        */
        splinePoints.pointsList.Add(newItem);
        viewIndex = splinePoints.pointsList.Count - 1;
    }

    void DeleteItem(int index)
    {
        splinePoints.pointsList.RemoveAt(index);
    }


    void DrawCommonControls()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Spline Editor", EditorStyles.boldLabel);
        GUILayout.Label(openedSplineAssetName, EditorStyles.textArea);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (splinePoints != null)
        {
            if (GUILayout.Button("Show Spline List"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = splinePoints;
            }
        }
        if (GUILayout.Button("Open Spline..."))
        {
            OpenItemList();
        }
        if (GUILayout.Button("New Spline"))
        {
            CreateNewSpline();
        }
        GUILayout.EndHorizontal();

        if (splinePoints == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Spline", GUILayout.ExpandWidth(false)))
            {
                CreateNewSpline();
            }
            if (GUILayout.Button("Open Existing Spline", GUILayout.ExpandWidth(false)))
            {
                OpenItemList();
            }
            GUILayout.EndHorizontal();
        }
    }

    void DrawSplineCommonControls()
    {
        GUILayout.BeginHorizontal();

        GUI.enabled = (splinePoints.pointsList != null); ;
        if (GUILayout.Button("Add Point", GUILayout.ExpandWidth(false)))
        {
            AddItem();
        }
        GUI.enabled = (splinePoints.pointsList != null) && (splinePoints.pointsList.Count > 0);
        if (GUILayout.Button("Delete Point", GUILayout.ExpandWidth(false)))
        {
            DeleteItem(viewIndex);
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        splinePoints.distance = EditorGUILayout.FloatField(new GUIContent("Distance", "Distance between new created point and last point from spline"), splinePoints.distance);
    }

    void DrawCurrentItemSelectionControls()
    {
        GUILayout.Label("Current Item", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUI.enabled = viewIndex > 0;
        if (GUILayout.Button(new GUIContent("<", "Prev point"), GUILayout.ExpandWidth(false)))
        {
            if (viewIndex > 0)
                viewIndex--;
        }
        GUI.enabled = true;

        GUI.enabled = viewIndex < splinePoints.pointsList.Count - 1;
        if (GUILayout.Button(new GUIContent(">", "Next point"), GUILayout.ExpandWidth(false)))
        {
            if (viewIndex < splinePoints.pointsList.Count - 1)
            {
                viewIndex++;
            }
        }
        GUI.enabled = true;

        viewIndex = EditorGUILayout.IntSlider(viewIndex, 0, splinePoints.pointsList.Count - 1);

        GUILayout.EndHorizontal();
    }

    void OnGUI()
    {
        DrawCommonControls();

        if (splinePoints != null)
        {
            DrawSplineCommonControls();

            if (splinePoints.pointsList.Count > 0)
            {
                DrawCurrentItemSelectionControls();

                
                for (int i = 0; i < splinePoints.pointsList[viewIndex].points.Count; i++)
                {
                    splinePoints.pointsList[viewIndex].points[i] = EditorGUILayout.Vector3Field("Position", splinePoints.pointsList[viewIndex].points[i]);
                    GUILayout.Space(5);
                }

                GUILayout.Space(10);
            }
            else
            {
                GUILayout.Label("This Path is Empty.");
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(splinePoints);
        }
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        if (splinePoints.pointsList.Count <= 0)
            return;

        //if (splinePoints.activeIndex < 0)
        //    return;

        //Transform handleTransform = new Transform();
        //Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Handles.color = Color.blue;
        LevelSplinePoint currPoint = null;
        LevelSplinePoint prevPoint = null;
        for (int i = 0; i < splinePoints.pointsList.Count; i++)
        {
            currPoint = splinePoints.pointsList[i];
            if (i > 0)
                prevPoint = splinePoints.pointsList[i - 1];

            if (currPoint != null)
            {
                for (int j = 0; j < currPoint.points.Count; j++)
                {
                    Vector3 position = currPoint.points[j];
                    Quaternion rotation = Quaternion.identity;

                    // Connect points with line
                    if (prevPoint != null)
                    {
                        
                        Vector3 prevPosition;

                        // Get available point from prev SlinePoint
                        if (j < prevPoint.points.Count)
                            prevPosition = prevPoint.points[j];
                        else
                        {
                            if (prevPoint.points.Count > 0)
                                prevPosition = prevPoint.points[prevPoint.points.Count - 1];
                            else
                                continue;
                        }

                        Handles.DrawLine(position, prevPosition);

                        // If we draw last point - connect all other points from prev point
                        if (j == currPoint.points.Count - 1)
                        {
                            for (int k = currPoint.points.Count; k < prevPoint.points.Count; k++)
                                Handles.DrawLine(position, prevPoint.points[k]);
                        }
                    }

                    Handles.SphereCap(j, position, rotation, 0.5f);

                    if (i == viewIndex)
                    {
                        EditorGUI.BeginChangeCheck();
                        position = Handles.DoPositionHandle(position, rotation);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(splinePoints, "Move Point");
                            EditorUtility.SetDirty(splinePoints);
                            currPoint.points[j] = position;
                        }
                            //ChangePointCoordinates(handleTransform.InverseTransformPoint(position));

                        /*EditorGUI.BeginChangeCheck();
                        rotation = Handles.RotationHandle(rotation, pointInWorldSpace);
                        if (EditorGUI.EndChangeCheck())
                            ChangePointRotation(rotation);*/
                    }
                }
            }
        }
    }
}
