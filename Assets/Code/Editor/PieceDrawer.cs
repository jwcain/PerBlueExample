using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Bestagon;

[CustomPropertyDrawer(typeof(Piece))]
public class PieceDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (position.WidthSubdivide(3f, out Rect[] positions, position.width * 0.66f, 64, null) == false)
        {
            base.OnGUI(position, property, label);
            return;
        }


        var hex = property.FindPropertyRelative("position");
        var isAnchor = property.FindPropertyRelative("isAnchor");

        EditorGUI.BeginProperty(position, label, property);
        {
            EditorGUI.PropertyField(positions[0], hex);
            //E.LabelField("isAnchor:", GUILayout.Width(16));
            EditorGUI.LabelField(positions[1], new GUIContent("isAnchor:"));
            EditorGUI.PropertyField(positions[2], isAnchor, new GUIContent(""));
        }
        EditorGUI.EndProperty();
    }
}
