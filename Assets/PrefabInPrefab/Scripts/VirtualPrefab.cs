using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class VirtualPrefab : MonoBehaviour
{
	void Awake()
	{
		if(!name.StartsWith(">NestedPrefab"))
		{
			Debug.LogError("This is dummy script.");
			DestroyImmediate(this);
			return;
		}
		if(Application.isPlaying) Destroy(gameObject);
	}

#if UNITY_EDITOR
	public GameObject stepparent;

	void Update()
	{
		if(Application.isPlaying) return;

		if(stepparent == null)
		{
			DestroyImmediate(gameObject);
			return;
		}
		this.transform.position = stepparent.transform.position;
		this.transform.rotation = stepparent.transform.rotation;
		this.transform.localScale = stepparent.transform.lossyScale; // set global scale
	}
#endif
}
