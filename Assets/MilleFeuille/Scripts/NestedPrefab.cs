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

	// ==============
	//  in edit mode
	// ==============

	void StartInEditMode()
	{
		DeleteChildren();

		var generatedObject = InstantiatePrefab();
		generatedObject.transform.parent = this.transform;
		generatedObject.hideFlags = HideFlags.HideAndDontSave;
	}

	void DeleteChildren()
	{
		for(int i = this.transform.childCount-1; i >= 0; --i)
		{
			DestroyImmediate(this.transform.GetChild(i).gameObject);
		}
	}
}
