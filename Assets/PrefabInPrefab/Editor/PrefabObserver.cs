using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace PrefabInPrefabAsset
{
public class PrefabObserver : UnityEditor.AssetModificationProcessor
	{
	static string[] OnWillSaveAssets (string[] paths)
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
		{
			if(!obj.name.StartsWith(">PrefabInPrefab")) continue;
			var component = obj.GetComponent<VirtualPrefab>().original;
			string prefabPath = component.GetPrefabFilePath();
			if(paths.Any(path => path == prefabPath)) component.ForceDrawDontEditablePrefab();
		}
		return paths;
	}
}
}
