using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PrefabInPrefabAsset
{

[ExecuteInEditMode]
public class VirtualPrefab : MonoBehaviour
{
	void Awake()
	{
		if(!name.StartsWith(">PrefabInPrefab"))
		{
			Debug.LogError("This is dummy script.");
			DestroyImmediate(this);
			return;
		}
		if(Application.isPlaying) Destroy(gameObject);
	}

#if UNITY_EDITOR
	public GameObject stepparent;
	public PrefabInPrefab original;

	void Update()
	{
		UpdateTransform();
	}

	public void UpdateTransform()
	{
		if(Application.isPlaying) return;

		if(stepparent == null)
		{
			DestroyImmediate(gameObject);
			return;
		}
		this.transform.position = stepparent.transform.position;
		this.transform.rotation = stepparent.transform.rotation;
		this.transform.localScale = stepparent.transform.localScale;

		var virtualPrefabs = GetChildVirtualPrefabs();
		foreach(var virtualPrefab in virtualPrefabs)
		{
			virtualPrefab.UpdateTransform();
		}
	}

	List<VirtualPrefab> GetChildVirtualPrefabs()
	{
		var virtualPrefabs = new List<VirtualPrefab>();
		var prefabInPrefabs = GetComponentsInChildren<PrefabInPrefab>(true);
		foreach(var prefabInPrefab in prefabInPrefabs)
		{
			if(prefabInPrefab.Child == null) continue;
			virtualPrefabs.Add(prefabInPrefab.Child.GetComponent<VirtualPrefab>());
		}
		return virtualPrefabs;
	}
#endif
}

}
