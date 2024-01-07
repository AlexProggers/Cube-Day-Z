using System.Collections;
using UnityEngine;

public class BattleRoyaleDisableAfter : MonoBehaviour
{
	[SerializeField]
	private Renderer _textRender;

	[SerializeField]
	private tk2dTextMesh _textMesh;

	public void ShowPlayerRemain(string msg)
	{
		_textRender.enabled = true;
		_textMesh.text = msg;
		StopCoroutine("DisableRenderer");
		StartCoroutine("DisableRenderer");
	}

	private IEnumerator DisableRenderer()
	{
		Color color = _textMesh.color;
		color.a = 1f;
		while (color.a > 0f)
		{
			color.a -= 0.04f;
			_textMesh.color = color;
			yield return new WaitForSeconds(0.1f);
		}
		_textRender.enabled = false;
	}
}
