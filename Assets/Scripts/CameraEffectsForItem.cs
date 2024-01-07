using System.Collections.Generic;
using Aubergine;
using UnityEngine;

public enum ItemCameraEffects
{
	Nightvision = 0
}

public class CameraEffectsForItem : ItemEffectInitializer
{
	[SerializeField]
	private ItemCameraEffects _effect;

	private List<MonoBehaviour> _effects = new List<MonoBehaviour>();

	public override void Initialize()
	{
		if (!(Camera.main == null) && _effect == ItemCameraEffects.Nightvision)
		{
			PP_NightVisionV2 componentInChildren = Camera.main.GetComponentInChildren<PP_NightVisionV2>();
			if ((bool)componentInChildren)
			{
				componentInChildren.enabled = true;
				_effects.Add(componentInChildren);
			}
		}
	}

	private void OnDestroy()
	{
		if (_effects.Count <= 0)
		{
			return;
		}
		foreach (MonoBehaviour effect in _effects)
		{
			if ((bool)effect)
			{
				effect.enabled = false;
			}
		}
		_effects.Clear();
	}
}
