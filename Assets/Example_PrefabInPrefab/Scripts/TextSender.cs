using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextSender : MonoBehaviour {
	[SerializeField] string text;

	void Start()
	{
		var receiver = GetComponent<TextReceiver>();
		if(receiver != null) receiver.text = text;
	}
}
