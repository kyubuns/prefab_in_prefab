using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PrefabInPrefab))]
public class PrefabInPrefabEditor : Editor {
	private SerializedProperty prefab;

	void OnEnable()
	{
		prefab = serializedObject.FindProperty("prefab");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		prefab.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab.objectReferenceValue, typeof(GameObject), false);
		serializedObject.ApplyModifiedProperties();
	}
}
