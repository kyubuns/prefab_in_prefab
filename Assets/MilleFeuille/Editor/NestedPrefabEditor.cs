using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(NestedPrefab))]
public class NestedPrefabEditor : Editor {
	public override void OnInspectorGUI() {
		NestedPrefab obj = target as NestedPrefab;
		obj.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", obj.prefab, typeof(GameObject), false);
		if(GUI.changed) EditorUtility.SetDirty(target);
	}
}
