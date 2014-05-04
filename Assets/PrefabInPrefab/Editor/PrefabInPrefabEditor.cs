using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PrefabInPrefab))]
public class PrefabInPrefabEditor : Editor {
	private SerializedProperty prefab;
	private SerializedProperty moveComponents;

	void OnEnable()
	{
		prefab = serializedObject.FindProperty("prefab");
		moveComponents = serializedObject.FindProperty("moveComponents");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		prefab.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab.objectReferenceValue, typeof(GameObject), false);
		moveComponents.boolValue = EditorGUILayout.Toggle("Move Components", moveComponents.boolValue);
		serializedObject.ApplyModifiedProperties();
	}
}
