using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PrefabInPrefab
{

[ExecuteInEditMode]
public class PrefabInPrefab : MonoBehaviour
{
	public GameObject Child { get { return generatedObject; } }

	[SerializeField] GameObject prefab;
	[SerializeField] bool moveComponents;
	private GameObject generatedObject;

	void Awake()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			StartInEditMode();
			return;
		}
#endif
		if(prefab == null) return;
		InstantiatePrefab();
	}

	GameObject InstantiatePrefab()
	{
		generatedObject = Instantiate(prefab) as GameObject;

		// calc transform to base parent
		generatedObject.transform.parent = this.transform.parent.transform;
		generatedObject.transform.position = this.transform.position;
		generatedObject.transform.rotation = this.transform.rotation;
		generatedObject.transform.localScale = this.transform.localScale;

		// change parent
		generatedObject.transform.parent = this.transform;

		if(moveComponents) MoveComponents(this.gameObject, generatedObject);
		return generatedObject;
	}

	void MoveComponents(GameObject from, GameObject to)
	{
		var components = from.GetComponents(typeof(Component));
		foreach(var component in components)
		{
			if(component as Transform != null || component as PrefabInPrefab != null) continue;

			Type type = component.GetType();
			var copy = to.AddComponent(type);
			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var field in fields) field.SetValue(copy, field.GetValue(component));
			if(Application.isPlaying) Destroy(component);
		}
	}

#if UNITY_EDITOR
	// ==============
	//  in edit mode
	// ==============

	private static int Redraw = 0;
	private static bool updateGameView = false;
	private DateTime lastPrefabUpdateTime;
	private int redrawCount = 0;
	private bool redrawForce = false;
	[SerializeField] bool previewInEditor = true;

	private bool visibleVirtualPrefab
	{
		get { return previewInEditor && this.gameObject.activeInHierarchy && this.enabled; }
	}

	public static void RequestRedraw()
	{
		if(Application.isPlaying) return;
		EditorApplication.delayCall += () =>
		{
			Redraw++;
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			SceneView.RepaintAll();
		};
	}

	void StartInEditMode()
	{
		DrawDontEditablePrefab();
	}

	void OnRenderObject()
	{
		if(Application.isPlaying) return;
		DrawDontEditablePrefab();
	}

	void OnDisable()
	{
		if(Application.isPlaying || redrawForce) return;
		DrawDontEditablePrefab();
	}

	void DrawDontEditablePrefab()
	{
		// param changed
		if((prefab == null || !visibleVirtualPrefab) && Child != null)
		{
			DeleteChildren();
			UpdateGameView();
			redrawCount = -1;
			return;
		}

		if(prefab == null || !visibleVirtualPrefab) return;
		if(Redraw == redrawCount && !PrefabUpdated()) return;
		if(ValidationError()) return;
		redrawCount = Redraw;

		DeleteChildren();

		var generatedObject = InstantiatePrefab();
		// 自分の1つ上のGameObjectが所属しているPrefabのRootの親の下.
		var foundRoot = PrefabUtility.FindPrefabRoot(transform.parent.gameObject).transform.parent;
		if(foundRoot == null)
		{
			// 親オブジェクトは、ドラッグアンドドロップした瞬間は見つからない
			EditorApplication.delayCall += () =>
			{
				if(generatedObject == null) return;
				generatedObject.transform.parent = PrefabUtility.FindPrefabRoot(transform.parent.gameObject).transform.parent.transform;
			};
		}
		else
		{
			generatedObject.transform.parent = foundRoot.transform;
		}
		generatedObject.name = string.Format(">PrefabInPrefab{0}", GetInstanceID());
		generatedObject.tag = "EditorOnly";
		foreach(var childTransform in generatedObject.GetComponentsInChildren<Transform>())
		{
			childTransform.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}

		var child = generatedObject.AddComponent<VirtualPrefab>();
		child.stepparent = this.gameObject;
		child.UpdateTransform();

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
			if(obj.name != string.Format(">PrefabInPrefab{0}", GetInstanceID())) continue;
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
			SceneView.RepaintAll();
			updateGameView = false;

			// force redraw anything(ex. NGUI's UICamera)
			var memo = this.gameObject.activeSelf;
			redrawForce = true;
			this.gameObject.SetActive(!memo);
			this.gameObject.SetActive(memo);
			redrawForce = false;
		};
	}

	bool ValidationError()
	{
		// check circular reference
		// って言うらしい。かっこいい。.
		if(CheckCircularReference(this, null))
		{
			Debug.LogError("Can't circular reference.");
			Reset();
			return true;
		}

		// This game object can't be root.
		// Because this is not in prefab.
		if(this.transform.parent == null)
		{
			// copy&paseした時に、なぜか一瞬だけparentがnullになるので、
			// 少し遅らせる.
			EditorApplication.delayCall += () =>
			{
				if(this.transform.parent == null)
				{
					Debug.LogError("Can't attach PrefabInPrefab to root gameobject.");
					Reset();
				}
				else
				{
					redrawCount = -1; //force redraw
					DrawDontEditablePrefab();
				}
			};

			// stop
			return true;
		}

		return false;
	}

	void Reset()
	{
		prefab = null;
		DeleteChildren();
	}

	bool CheckCircularReference(PrefabInPrefab target, List<int> usedPrefabs)
	{
		if(target.prefab == null) return false;
		if(usedPrefabs == null) usedPrefabs = new List<int>();

		int id = target.prefab.GetInstanceID();
		if(usedPrefabs.Contains(id)) return true;
		usedPrefabs.Add(id);

		foreach(var nextTarget in ((GameObject)target.prefab).GetComponentsInChildren<PrefabInPrefab>(true))
		{
			if(nextTarget == this) continue;
			if(CheckCircularReference(nextTarget, usedPrefabs)) return true;
		}

		return false;
	}
#endif
}

}
