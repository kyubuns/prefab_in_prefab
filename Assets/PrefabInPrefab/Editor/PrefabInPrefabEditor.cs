using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PrefabInPrefab))]
public class PrefabInPrefabEditor : Editor {
	public override void OnInspectorGUI() {
		var obj = target as PrefabInPrefab;
		obj.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", obj.prefab, typeof(GameObject), false);
		if(GUI.changed) EditorUtility.SetDirty(target);
	}
}
