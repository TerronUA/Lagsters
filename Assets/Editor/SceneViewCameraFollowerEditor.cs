using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SceneViewCameraFollower))]
public class SceneViewCameraFollowerEditor : Editor
{
    private ArrayList sceneViews;
    private SceneView sceneView;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Use current position"))
        {
            SceneViewCameraFollower _target = target as SceneViewCameraFollower;
            sceneViews = SceneView.sceneViews;

            if (_target.sceneViewFollowers.Length > 0)
            {
                sceneView = (SceneView)sceneViews[0];

                _target.sceneViewFollowers[0].positionOffset = sceneView.camera.transform.position - _target.sceneViewFollowers[0].targetTransform.position;
                _target.sceneViewFollowers[0].fixedRotation = sceneView.rotation.eulerAngles;
            }
        }
    }
}
