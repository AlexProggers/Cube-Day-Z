using UnityEngine;
using UnityEngine.UI;

public class SplashChanger : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Sprite _newTexture;

	private void Awake()
	{
		_image.overrideSprite = _newTexture;
		if (Application.absoluteURL.Contains("play_fb") || Application.absoluteURL.Contains("facebook"))
		{
			_image.overrideSprite = _newTexture;
		}
	}
}
