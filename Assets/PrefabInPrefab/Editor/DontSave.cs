using UnityEngine;
using UnityEditor;

namespace PrefabInPrefab
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
			obj.transform.parent = null;
		}
		return paths;
	}
}
}
