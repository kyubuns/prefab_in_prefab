using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class NestedPrefab : MonoBehaviour {
	public GameObject prefab;

	void Start()
	{
		if(!Application.isPlaying) return;

		InstantiatePrefab();
		Destroy(this.gameObject);
	}

	GameObject InstantiatePrefab()
	{
		var generatedObject = Instantiate(prefab) as GameObject;

		generatedObject.transform.parent = this.transform.parent.transform;
		generatedObject.transform.position = this.transform.position;
		generatedObject.transform.rotation = this.transform.rotation;
		generatedObject.transform.localScale = this.transform.localScale;
		generatedObject.name = this.name;

		return generatedObject;
	}

#if UNITY_EDITOR
	// ==============
	//  in edit mode
	// ==============

	private DateTime lastPrefabUpdateTime;

	void DrawDontEditablePrefab()
	{
		Debug.Log("DrawDontEditablePrefab");
		DeleteChildren();

		var generatedObject = InstantiatePrefab();
		generatedObject.transform.parent = this.transform;
		generatedObject.hideFlags = HideFlags.HideAndDontSave;

		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		lastPrefabUpdateTime = GetPrefabUpdateTime();
	}

	void DeleteChildren()
	{
		for(int i = this.transform.childCount-1; i >= 0; --i)
		{
			DestroyImmediate(this.transform.GetChild(i).gameObject);
		}
	}

	void OnRenderObject()
	{
		if(Application.isPlaying) return;

		var prefabUpdateTime = GetPrefabUpdateTime();
		if(lastPrefabUpdateTime == prefabUpdateTime) return;

		DrawDontEditablePrefab();
	}

	DateTime GetPrefabUpdateTime()
	{
		var path = AssetDatabase.GetAssetPath(prefab);
		var lastUpdateTime = System.IO.File.GetLastWriteTime(path);
		return lastUpdateTime;
	}
#endif
}
