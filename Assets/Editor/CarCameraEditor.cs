using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(CarCamera))]
public class CarCameraEditor : Editor
{
    private ArrayList sceneViews;
//    private SceneView sceneView;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Use current viev"))
        {
            CarCamera _target = target as CarCamera;
            sceneViews = SceneView.sceneViews;

            if (_target.follow)
            {
//                sceneView = (SceneView)sceneViews[0];

//                _target.sceneViewFollowers[0].positionOffset = sceneView.camera.transform.position - _target.follow.transform.position;
//                _target.sceneViewFollowers[0].fixedRotation = sceneView.rotation.eulerAngles;
            }
        }
    }
}
