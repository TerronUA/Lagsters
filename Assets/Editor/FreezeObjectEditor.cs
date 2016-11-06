using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FreezeObject))]
public class FreezeObjectEditor : Editor
{
    Tool LastTool = Tool.None;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CheckToolsAwailability();
    }

    void OnEnable()
    {

        LastTool = Tools.current;
        CheckToolsAwailability();
    }

    private void CheckToolsAwailability()
    {
        FreezeObject _target = target as FreezeObject;
        switch (Tools.current)
        {
            case Tool.View:
                break;
            case Tool.Move:
                if (_target.FreezePosition)
                    Tools.current = Tool.None;
                break;
            case Tool.Rotate:
                if (_target.FreezeRotation)
                    Tools.current = Tool.None;
                break;
            case Tool.Scale:
                break;
            case Tool.Rect:
                break;
            case Tool.None:
                break;
            default:
                break;
        }
    }

    void OnDisable()
    {
        Tools.current = LastTool;
    }
}
