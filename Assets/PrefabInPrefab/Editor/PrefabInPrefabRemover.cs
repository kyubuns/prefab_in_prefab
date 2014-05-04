using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PrefabInPrefab
{

public class PrefabInPrefabRemover : UnityEditor.AssetModificationProcessor {
	public static bool Removed { get; private set; }

	static string[] OnWillSaveAssets (string[] paths) {
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
		{
			if(!obj.name.StartsWith(">PrefabInPrefab")) continue;
			Object.DestroyImmediate(obj);
			PrefabInPrefab.RequestRedraw();
		}
		return paths;
	}
}

}
