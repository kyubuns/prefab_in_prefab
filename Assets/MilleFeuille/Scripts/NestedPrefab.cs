using UnityEngine;
using System.Collections;

public class NestedPrefab : MonoBehaviour {
	[SerializeField] GameObject prefab;

	void Start()
	{
		GameObject generatedObject = Instantiate(prefab) as GameObject;

		generatedObject.transform.parent = this.transform.parent.transform;
		generatedObject.transform.position = this.transform.position;
		generatedObject.transform.rotation = this.transform.rotation;
		generatedObject.transform.localScale = this.transform.localScale;

		generatedObject.name = this.name;

		Destroy(this.gameObject);
	}
}
