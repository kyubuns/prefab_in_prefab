using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class NestedPrefab : MonoBehaviour
{
	public GameObject prefab;

	void Start()
	{
		if(!Application.isPlaying)
		{
			StartInEditMode();
			return;
		}

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

	void StartInEditMode()
	{
		DrawDontEditablePrefab();
	}

	void OnRenderObject()
	{
		if(Application.isPlaying) return;

		DrawDontEditablePrefab();
	}

	void DrawDontEditablePrefab()
	{
		if(prefab == null) return;

		var prefabUpdateTime = GetPrefabUpdateTime();
		if(lastPrefabUpdateTime == prefabUpdateTime) return;

		Debug.Log(string.Format("DrawDontEditablePrefab - {0}", prefab));
		DeleteChildren();

		var generatedObject = InstantiatePrefab();
		generatedObject.transform.parent = null;
		generatedObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		generatedObject.tag = "EditorOnly";
		generatedObject.name = string.Format("NestedPrefab-{0}", GetInstanceID());
		generatedObject.AddComponent<NestedPrefabChild>();

		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		lastPrefabUpdateTime = GetPrefabUpdateTime();
	}

	void DeleteChildren()
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
		{
			if(obj.name != string.Format("NestedPrefab-{0}", GetInstanceID())) continue;
			DestroyImmediate(obj);
		}
	}

	string GetFilePath()
	{
		return AssetDatabase.GetAssetPath(prefab);
	}

	DateTime GetPrefabUpdateTime()
	{
		return System.IO.File.GetLastWriteTime(GetFilePath());
	}
#endif
}
