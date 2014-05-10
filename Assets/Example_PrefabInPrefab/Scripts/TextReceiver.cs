using UnityEngine;
using System.Collections;

public class TextReceiver : MonoBehaviour {
	[SerializeField] TextMesh textMesh;

	public string text
	{
		get
		{
			return textMesh.text;
		}
		set
		{
			textMesh.text = value;
		}
	}
}
