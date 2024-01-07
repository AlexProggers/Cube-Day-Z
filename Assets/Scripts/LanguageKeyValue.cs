using UnityEngine;

public class LanguageKeyValue : MonoBehaviour
{
	public string RussianValue;

	public string EnglishValue;

	public bool fixOnMobile;

	public string mobile_RussianValue;

	public string mobile_EnglishValue;

	public bool ReplaceStarSimbol;

	private tk2dTextMesh _label;

	private void Awake()
	{
		if (ReplaceStarSimbol)
		{
			RussianValue = RussianValue.Replace("*", "\n");
			EnglishValue = EnglishValue.Replace("*", "\n");
		}
		_label = GetComponent<tk2dTextMesh>();
		LocalizationController.Instance.AddLanguageLabel(this);
	}

	private void OnDestroy()
	{
		LocalizationController.Instance.RemoveLanguageLabel(this);
	}

	public void UpdateLabel()
	{
		switch (DataKeeper.Language)
		{
		case Language.Russian:
			if (!fixOnMobile)
			{
				_label.text = RussianValue;				
			}
			else
			{
				_label.text = mobile_RussianValue;
			}
			break;
		case Language.English:
			if (!fixOnMobile)
			{
				_label.text = EnglishValue;
			}
			else
			{
				_label.text = mobile_EnglishValue;
			}
			break;
		}
	}
}
