using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PathPointsEditor : EditorWindow
{
    public PathPoints pathPoints;
    private int viewIndex = 0;

    [MenuItem("Window/Lagsters/Path Points Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow<PathPointsEditor>("Path Editor");
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            pathPoints = AssetDatabase.LoadAssetAtPath(objectPath, typeof(PathPoints)) as PathPoints;
        }
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void CreateNewItemList()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        viewIndex = 0;
        pathPoints = PathPointsCreator.Create();
        if (pathPoints)
        {
            pathPoints.pointsList = new List<PathPoint>();
            string relPath = AssetDatabase.GetAssetPath(pathPoints);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void OpenItemList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Path List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            pathPoints = AssetDatabase.LoadAssetAtPath(relPath, typeof(PathPoints)) as PathPoints;
            if (pathPoints.pointsList == null)
                pathPoints.pointsList = new List<PathPoint>();
            if (pathPoints)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    void AddItem()
    {
        PathPoint newItem = new PathPoint();
        newItem.position = Vector3.zero;
        newItem.rotation = Quaternion.identity;

        pathPoints.pointsList.Add(newItem);
        viewIndex = pathPoints.pointsList.Count - 1;
    }

    void DeleteItem(int index)
    {
        pathPoints.pointsList.RemoveAt(index);
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Path Editor", EditorStyles.boldLabel);
        if (pathPoints != null)
        {
            if (GUILayout.Button("Show Path List"))
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = pathPoints;
            }
        }
        if (GUILayout.Button("Open Path..."))
        {
            OpenItemList();
        }
        if (GUILayout.Button("New Path"))
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = pathPoints;
        }
        GUILayout.EndHorizontal();

        if (pathPoints == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Path", GUILayout.ExpandWidth(false)))
            {
                CreateNewItemList();
            }
            if (GUILayout.Button("Open Existing Path", GUILayout.ExpandWidth(false)))
            {
                OpenItemList();
            }
            GUILayout.EndHorizontal();
        }

        if (pathPoints != null)
        {
            pathPoints.layer = EditorGUILayout.LayerField("Layer", pathPoints.layer);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex > 0)
                    viewIndex--;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
            {
                if (viewIndex < pathPoints.pointsList.Count - 1)
                {
                    viewIndex++;
                }
            }

            GUILayout.Space(60);

            GUI.enabled = (pathPoints.pointsList != null); ;
            if (GUILayout.Button("Add Point", GUILayout.ExpandWidth(false)))
            {
                AddItem();
            }
            GUI.enabled = (pathPoints.pointsList != null) && (pathPoints.pointsList.Count > 0);
            if (GUILayout.Button("Delete Point", GUILayout.ExpandWidth(false)))
            {
                DeleteItem(viewIndex);
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            if (pathPoints.pointsList == null)
                Debug.Log("wtf");
            if (pathPoints.pointsList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 0, pathPoints.pointsList.Count - 1);
                //Mathf.Clamp (viewIndex, 1, pathPoints.itemList.Count);
                EditorGUILayout.LabelField("of   " + pathPoints.pointsList.Count.ToString() + "  items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                pathPoints.pointsList[viewIndex].position = EditorGUILayout.Vector3Field("Position", pathPoints.pointsList[viewIndex].position);
                pathPoints.pointsList[viewIndex].rotation.eulerAngles = EditorGUILayout.Vector3Field("Rotation", pathPoints.pointsList[viewIndex].rotation.eulerAngles);
                
                GUILayout.Space(10);
            }
            else
            {
                GUILayout.Label("This Path is Empty.");
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(pathPoints);
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (pathPoints.pointsList.Count <= 0)
            return;

        if (viewIndex < 0)
            return;

        int startDraw = viewIndex - pathPoints.drawPointsCount;
        int endDraw = viewIndex + pathPoints.drawPointsCount;
        if (startDraw < 0)
        {
            endDraw = endDraw + Mathf.Abs(startDraw);
            startDraw = 0;
        }
        if (endDraw > pathPoints.pointsList.Count - 1)
            endDraw = pathPoints.pointsList.Count - 1;

        //Transform handleTransform = _target.transform;
        Quaternion handleRotation = /*Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : */Quaternion.identity;

        Handles.color = Color.blue;
        for (int i = startDraw; i <= endDraw; i++)
        {
            if (pathPoints.pointsList[i] != null)
            {
                Vector3 pointInWorldSpace = /*handleTransform.TransformPoint(*/pathPoints.pointsList[i].position;//);
                Quaternion rotation = pathPoints.pointsList[i].rotation;

                // Connect points with line
                if (i > startDraw)
                {
                    Vector3 prevPointInWorldSpace = /*handleTransform.TransformPoint(*/pathPoints.pointsList[i - 1].position;//);
                    Handles.DrawLine(pointInWorldSpace, prevPointInWorldSpace);
                }

                Handles.SphereCap(i, pointInWorldSpace, handleRotation, 0.5f);

                if (i == viewIndex)
                {
                    EditorGUI.BeginChangeCheck();
                    pointInWorldSpace = Handles.DoPositionHandle(pointInWorldSpace, rotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        pathPoints.pointsList[i].position = pointInWorldSpace;
                    }
                        //ChangePointCoordinates(handleTransform.InverseTransformPoint(pointInWorldSpace));

                    EditorGUI.BeginChangeCheck();
                    rotation = Handles.RotationHandle(rotation, pointInWorldSpace);
                    if (EditorGUI.EndChangeCheck())
                    {
                        pathPoints.pointsList[i].rotation = rotation;
                    }
                     //   ChangePointRotation(rotation);
                }
            }
        }
    }

}
