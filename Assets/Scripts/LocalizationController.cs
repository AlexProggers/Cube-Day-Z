using System.Collections.Generic;

public class LocalizationController
{
	private static LocalizationController _instance;

	private List<LanguageKeyValue> _languageLabels = new List<LanguageKeyValue>();

	public static LocalizationController Instance
	{
		get
		{
			return _instance ?? (_instance = new LocalizationController());
		}
	}

	public void AddLanguageLabel(LanguageKeyValue label)
	{
		label.UpdateLabel();
		if (!_languageLabels.Contains(label))
		{
			_languageLabels.Add(label);
		}
	}

	public void RemoveLanguageLabel(LanguageKeyValue label)
	{
		if (_languageLabels.Contains(label))
		{
			_languageLabels.Remove(label);
		}
	}

	public void UpdateLabels()
	{
		foreach (LanguageKeyValue languageLabel in _languageLabels)
		{
			languageLabel.UpdateLabel();
		}
	}
}
