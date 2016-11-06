using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(GravityPoint))]
public class GravityPointDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		contentPosition.width *= 0.75f;
        int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("position"), GUIContent.none, true);
        contentPosition.x += contentPosition.width;
        contentPosition.width *= 0.3f;
        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("gravity"), new GUIContent("G"));
        EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
