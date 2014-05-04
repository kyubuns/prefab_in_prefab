using UnityEngine;
using UnityEditor;
using System.Collections;

public class PrefabInPrefabRemover : UnityEditor.AssetModificationProcessor {
	public static bool Removed { get; private set; }

	static string[] OnWillSaveAssets (string[] paths) {
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
		{
			if(!obj.name.StartsWith(">NestedPrefab")) continue;
			Object.DestroyImmediate(obj);
			EditorApplication.delayCall += () =>
			{
				PrefabInPrefab.Redraw++;
				UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
				SceneView.RepaintAll();
			};
		}
		return paths;
	}
}
