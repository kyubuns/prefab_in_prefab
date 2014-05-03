using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NestedPrefab : MonoBehaviour {
	public GameObject prefab;

	void Start()
	{
		if(!Application.isPlaying)
		{
			StartInEditMode();
			return;
		}

		Debug.Log("Start");
		InstantiatePrefab();

		Destroy(this.gameObject);
	}

	void Update()
	{
		if(!Application.isPlaying)
		{
			UpdateInEditMode();
			return;
		}
	}

	void OnDestroy()
	{
		if(!Application.isPlaying)
		{
			OnDestroyInEditMode();
			return;
		}

		Debug.Log("OnDestroy");
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

	// ==============
	//  in edit mode
	// ==============

	void StartInEditMode()
	{
		Debug.Log("Start in Edit Mode");
		Debug.Log(this.transform.childCount);
		for(int i = this.transform.childCount-1; i >= 0; --i)
		{
			Debug.Log(i);
			DestroyImmediate(this.transform.GetChild(i).gameObject);
		}

		var generatedObject = InstantiatePrefab();
		generatedObject.transform.parent = this.transform;
		generatedObject.hideFlags = HideFlags.HideAndDontSave;
	}

	void OnDestroyInEditMode()
	{
		Debug.Log("OnDestroy In Edit Mode");
	}

	void UpdateInEditMode()
	{
		//Debug.Log("Update In Edit Mode");
	}
}
