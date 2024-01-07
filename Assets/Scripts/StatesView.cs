using System.Collections.Generic;
using UnityEngine;

public class StatesView : MonoBehaviour
{
	private const float MinDesaturateValue = 0f;

	private const float MaxDesaturateValue = 1.1f;

	public static StatesView I;

	[SerializeField]
	private List<ManState> _statesKeys;

	[SerializeField]
	private List<ProgressView> _statesViews;

	private ProgressView _stamina;

	private Camera _weaponCamera;

	private void Awake()
	{
		I = this;
		if (_statesKeys.Contains(ManState.Stamina))
		{
			int num = _statesKeys.FindIndex((ManState s) => s == ManState.Stamina);
			if (num > 0 && num < _statesViews.Count)
			{
				_stamina = _statesViews[num];
			}
		}
	}

	private float GetDesaturateValue(float factor)
	{
		return 1.1f - 1.1f * (1f - factor);
	}

	public void UpdateState(ManState state, float value)
	{
		if (!_statesKeys.Contains(state))
		{
			return;
		}
		int num = _statesKeys.FindIndex((ManState s) => s == state);
		if (num < 0 || num >= _statesViews.Count)
		{
			return;
		}
		_statesViews[num].UpdateView(value);
		if (state == ManState.HealthPoint)
		{
			float factor = Mathf.Max(0f, Mathf.Min(DataKeeper.MaxPlayerHp, value)) / 100f;
			if (GameControls.I != null && GameControls.I.Player != null)
			{
				GameControls.I.Player.weaponCameraObj.SendMessage("SetDesaturateValue", GetDesaturateValue(factor));
			}
		}
	}

	private void Update()
	{
		if (!(_stamina == null) && !(GameControls.I == null) && !(GameControls.I.Walker == null))
		{
			_stamina.UpdateView((int)(GameControls.I.Walker.staminaForSprintAmt / (float)GameControls.I.Walker.staminaForSprint * 100f));
		}
	}
}
