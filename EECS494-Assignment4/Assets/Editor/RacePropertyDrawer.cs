using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Race))]
public class RacePropertyDrawer : PropertyDrawer {

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		if(!property.isExpanded) return 16f;

		SerializedProperty towers = property.FindPropertyRelative("towerMapKey");
		SerializedProperty creeps = property.FindPropertyRelative("creepMapKey");

		return 16f + 
				(!towers.isExpanded ? 16f : 32f + 16f * towers.arraySize) +
				(!creeps.isExpanded ? 16f : 32f + 16f * creeps.arraySize);
	}


	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		position.height = 16f;
		Rect foldoutPosition = position;
		foldoutPosition.x -= 14f;
		foldoutPosition.width += 14f;

		label = EditorGUI.BeginProperty(position, label, property);
		property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, label, true);
		EditorGUI.EndProperty();
		
		if (!property.isExpanded) {
			return;
		}
		SerializedProperty towerKeys = property.FindPropertyRelative("towerMapKey");
		SerializedProperty towerValues = property.FindPropertyRelative("towerMapValue");

		position.y += 16f;
		position.width /= 2f;

		towerKeys.isExpanded = EditorGUI.Foldout(position , towerKeys.isExpanded, "Towers", true);

		if(towerKeys.isExpanded)
		{
			position.y += 16f;
			EditorGUI.LabelField(position, "Size");
			position.x += position.width;
			int size = EditorGUI.IntField(position, towerKeys.arraySize);
			position.x -= position.width;

			towerKeys.arraySize = size;
			towerValues.arraySize = size;

			for(int i = 0; i < towerKeys.arraySize; i++)
			{
				position.y += 16f;
				SerializedProperty key = towerKeys.GetArrayElementAtIndex(i);
				key.stringValue = EditorGUI.TextField(position, key.stringValue);
				position.x += position.width;
				SerializedProperty value = towerValues.GetArrayElementAtIndex(i);
				value.objectReferenceValue = EditorGUI.ObjectField(position, value.objectReferenceValue, typeof(Tower), false);
				position.x -= position.width;
			}
		}

		SerializedProperty creepKeys = property.FindPropertyRelative("creepMapKey");
		SerializedProperty creepValues = property.FindPropertyRelative("creepMapValue");

		position.y += 16f;

		creepKeys.isExpanded = EditorGUI.Foldout(position, creepKeys.isExpanded, "Creeps", true);
		
		if(creepKeys.isExpanded)
		{
			position.y += 16f;
			EditorGUI.LabelField(position, "Size");
			position.x += position.width;
			int size = EditorGUI.IntField(position, creepKeys.arraySize);
			position.x -= position.width;

			creepKeys.arraySize = size;
			creepValues.arraySize = size;

			for(int i = 0; i < creepKeys.arraySize; i++)
			{
				position.y += 16f;
				SerializedProperty key = creepKeys.GetArrayElementAtIndex(i);
				key.stringValue = EditorGUI.TextField(position, key.stringValue);
				position.x += position.width;
				SerializedProperty value = creepValues.GetArrayElementAtIndex(i);
				value.objectReferenceValue = EditorGUI.ObjectField(position, value.objectReferenceValue, typeof(Creep), false);
				position.x -= position.width;
			}
		}

	}
}
