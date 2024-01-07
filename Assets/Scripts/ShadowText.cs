using UnityEngine;

public class ShadowText : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh _label;

	[SerializeField]
	private tk2dTextMesh _shadow;

	public void SetText(string text)
	{
		_label.text = text;
		_shadow.text = text;
	}
}
