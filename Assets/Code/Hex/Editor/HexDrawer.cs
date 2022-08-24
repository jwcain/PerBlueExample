using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bestagon.Hexagon
{
    [CustomPropertyDrawer(typeof(Hex))]
    public class HexDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.WidthSubdivide(3f, out Rect[] positions, 10f, null, 10f, null) == false)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var q = property.FindPropertyRelative("m_q");
            var r = property.FindPropertyRelative("m_r");
            EditorGUI.BeginProperty(position, label, property);
            {
                //Vector2Int t = EditorGUI.Vector2IntField(position, "", new Vector2Int(q.intValue, r.intValue));
                //q.intValue = t.x; r.intValue = t.y;
                EditorGUI.LabelField(positions[0], "Q");
                q.intValue = EditorGUI.IntField(positions[1], new GUIContent(), q.intValue);
                EditorGUI.LabelField(positions[2], "R");
                r.intValue = EditorGUI.IntField(positions[3], new GUIContent(), r.intValue);
            }
            EditorGUI.EndProperty();
        }


    }
}