using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(NavDirections))]
public class NavDirectionsDrawer : PropertyDrawer
{
	// public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	// {
	// 	EditorGUI.BeginProperty(position, label, property);
	//
	// 	// Draw label
	// 	position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
	// 	// Calculate rects
	// 	//
	// 	float s = 20;
	// 	
	// 	//
	// 	var upRect = new Rect(position.x+s, position.y-position.height, 30, position.height*3);
	// 	// var rightRect = new Rect(position.x + 2*s, position.y-s, 30, position.height*2);
	// 	var downRect = new Rect(position.x + s, position.y, 30, position.height*3);
	// 	// var leftRect = new Rect(position.x, position.y-s, 30, position.height*2);
	// 	
	//
	// 	// Draw fields - passs GUIContent.none to each so they are drawn without labels
	// 	// EditorGUI.PropertyField(upRect, property.FindPropertyRelative("OpenUp"), GUIContent.none);
	// 	// EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("OpenRight"), GUIContent.none);
	// 	// EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("OpenDown"), GUIContent.none);
	// 	 // EditorGUI.PropertyField(downRect, property.FindPropertyRelative("OpenLeft"), GUIContent.none);
	//
	//
	// 	EditorGUI.EndProperty();
	// }
}
