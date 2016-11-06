using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(LevelBlock))]
public class LevelBlockEditor : Editor
{
    Vector3 normalForTarget;
    Vector3 normalForNextBlock;
    public override void OnInspectorGUI()
    {
        LevelBlock _target = target as LevelBlock;
        base.OnInspectorGUI();

        if (GUILayout.Button("Realign"))
        {
            normalForTarget = Vector3.Cross((_target.endPoint1.position - _target.endCenter.position),
                                            (_target.endPoint2.position - _target.endCenter.position)).normalized;
            normalForNextBlock = Vector3.Cross((_target.nextBlockPoint1.position - _target.nextBlockCenter.position),
                                               (_target.nextBlockPoint2.position - _target.nextBlockCenter.position)).normalized;
            float angle = Vector3.Angle(normalForTarget, normalForNextBlock);
            Debug.Log(angle);
            if (Math.Abs(angle - 180f) > 0.01)
            {
                //_target.nextBlock.transform.rotation = Quaternion.FromToRotation(normalForNextBlock, -normalForTarget);
            }

            float distance = Vector3.Distance(_target.nextBlockCenter.position, _target.endCenter.position);
            if (distance > 0.003)
                _target.nextBlock.transform.position = _target.nextBlock.transform.position + (_target.endCenter.position - _target.nextBlockCenter.position);
        }
        Debug.DrawRay(_target.endCenter.position, normalForTarget, Color.red);
        //Debug.DrawRay(_target.nextBlockCenter.position, _target.nextBlockCenter.up, Color.magenta);
        //Debug.DrawRay(_target.nextBlockCenter.position, normalForNextBlock, Color.green);
    }
}
