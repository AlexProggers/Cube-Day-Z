using UnityEngine;

public class ProgressView : MonoBehaviour
{
	[SerializeField]
	private tk2dUIProgressBar _progress;

	[SerializeField]
	private tk2dTextMesh _text;

	public void UpdateView(float progress)
	{
		_progress.Value = progress / 100f;
		_text.text = (int)progress + "%";
	}
}
