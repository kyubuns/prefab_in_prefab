using UnityEngine;
using UnityEditor;

namespace PrefabInPrefabAsset
{
//Unityのバグによって、DontSaveフラグを建てたゲームオブジェクトのゴミが保存されてしまう.
//http://answers.unity3d.com/questions/609621/hideflagsdontsave-causes-checkconsistency-transfor.html
public class DontSave : UnityEditor.AssetModificationProcessor
{
	static string[] OnWillSaveAssets (string[] paths)
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
		{
			if(!obj.name.StartsWith(">PrefabInPrefab")) continue;
			var originalParent = obj.transform.parent;
			foreach(var childTransform in obj.GetComponentsInChildren<Transform>())
			{
				childTransform.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
			obj.transform.parent = null;

			EditorApplication.delayCall += () =>
			{
				if(obj == null) return;
				obj.transform.parent = originalParent;
				foreach(var childTransform in obj.GetComponentsInChildren<Transform>())
				{
					childTransform.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
				}
			};

		}
		return paths;
	}
}
}
