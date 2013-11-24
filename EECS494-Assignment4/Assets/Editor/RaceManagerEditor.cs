using UnityEditor;
using UnityEngine;
//
//[CustomEditor(typeof(RaceManager))]
//public class RaceManagerEditor : Editor {
//	public override void OnInspectorGUI ()
//	{
//		EditorGUI.indentLevel +=1;
//
//		SerializedProperty raceKeys = serializedObject.FindProperty("raceMapKeys");
//		SerializedProperty raceValues = serializedObject.FindProperty("raceMapValues");
//
//		EditorGUILayout.BeginVertical();
//		raceKeys.isExpanded = EditorGUILayout.Foldout(raceKeys.isExpanded, "Races");
//		EditorGUILayout.EndVertical();
//
//		if(raceKeys.isExpanded)
//		{
//			EditorGUILayout.BeginVertical();
//			EditorGUILayout.BeginHorizontal();
//			EditorGUILayout.LabelField("Size");
//			raceKeys.arraySize = raceValues.arraySize = EditorGUILayout.IntField(raceKeys.arraySize);
//			EditorGUILayout.EndHorizontal();
//			EditorGUI.indentLevel+=1;
//			for(int i = 0; i < raceKeys.arraySize; i++)
//			{
//				SerializedProperty key = raceKeys.GetArrayElementAtIndex(i);
//				SerializedProperty value = raceValues.GetArrayElementAtIndex(i);
//
//				EditorGUILayout.BeginHorizontal();
//				EditorGUILayout.LabelField("Name");
//				key.stringValue = EditorGUILayout.TextField(key.stringValue);
//				EditorGUILayout.EndHorizontal();
//				EditorGUILayout.PropertyField(value, new GUIContent(key.stringValue));
//			}
//
//			EditorGUILayout.EndVertical();
//			EditorGUI.indentLevel-=1;
//		}
//		EditorGUI.indentLevel-=1;
//
//		if(GUI.changed)
//		{
//			EditorUtility.SetDirty(target);
//		}
//	}
//}
