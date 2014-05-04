#define DEBUG

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

	void Awake()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			StartInEditMode();
			return;
		}
#endif

		if(prefab != null)
		{
			InstantiatePrefab();
			Destroy(this.gameObject);
		}
		else
		{
			// don't delete game object when prefab is null.
			Destroy(this);
		}
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

	public static int Redraw = 0;
	private static bool updateGameView = false;
	private DateTime lastPrefabUpdateTime;
	private int redrawCount = 0;

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
		if(Redraw == redrawCount && !PrefabUpdated()) return;
		if(ValidationError()) return;
		redrawCount = Redraw;

#if DEBUG
		Debug.Log("DrawDontEditablePrefab");
#endif
		DeleteChildren();

		var generatedObject = InstantiatePrefab();
		generatedObject.transform.parent = null;
#if DEBUG
		generatedObject.hideFlags = HideFlags.NotEditable;
#else
		generatedObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif
		generatedObject.tag = "EditorOnly";
		generatedObject.name = string.Format(">NestedPrefab{0}", GetInstanceID());

		var child = generatedObject.AddComponent<NestedPrefabDummyChild>();
		child.stepparent = this.gameObject;

		UpdateGameView();
	}

	bool PrefabUpdated()
	{
		var prefabUpdateTime = GetPrefabUpdateTime();
		if(lastPrefabUpdateTime == prefabUpdateTime) return false;
		lastPrefabUpdateTime = GetPrefabUpdateTime();
		return true;
	}

	void DeleteChildren()
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EditorOnly"))
		{
			if(obj.name != string.Format(">NestedPrefab{0}", GetInstanceID())) continue;
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

	void UpdateGameView()
	{
		if(updateGameView) return;
		updateGameView = true;
		EditorApplication.delayCall += () =>
		{
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			updateGameView = false;
		};
	}

	bool ValidationError()
	{
#if DEBUG
		Debug.Log("ValidationError");
#endif

		// 1.
		// This game object can't have any other components.
		// Because this game object will delete in Start() in play mode.
		foreach(var component in this.gameObject.GetComponents(typeof(Component)))
		{
			if(component as NestedPrefab == null && component as Transform == null)
			{
				Debug.LogError("Nested Prefab's game object can't have any other components.");
				DestroyImmediate(component);
			}
		}
		
		// 2.
		// This game object can't have child.
		// For the same reason.
		if(this.transform.childCount > 0)
		{
			Debug.LogError("Nested Prefab's game object can't have child.");
			for(int i=this.transform.childCount-1; i>=0; --i)
			{
				DestroyImmediate(this.transform.GetChild(i).gameObject);
			}
		}

		// 3.
		// Prefab in Prefab in Prefab
		// any problems.
		// ex. A in B in A in B in ...
		var nestedPrefabs = ((GameObject)prefab).GetComponentsInChildren<NestedPrefab>(true);
		if(nestedPrefabs.Length > 0)
		{
			Debug.LogError("Can't prefab in prefab in prefab.");
			prefab = null;
			DeleteChildren();
			return true;
		}

		// 4.
		// This game object can't be root.
		// Because this is not in prefab.
		if(this.transform.parent == null)
		{
			EditorApplication.delayCall += () =>
			{
				if(this.transform.parent == null)
				{
					Debug.LogError("Can't attach NestedPrefab to root gameobject.");
					prefab = null;
					DeleteChildren();
				}
				else
				{
					redrawCount--; //force redraw
					DrawDontEditablePrefab();
				}
			};

			// stop
			return true;
		}

		return false;
	}
#endif
}
