using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class HowManyWindowController : MonoBehaviour
{
	[SerializeField]
	private tk2dUIProgressBar _howManyProgress;

	[SerializeField]
	private tk2dUIScrollbar _howManyScrollbar;

	[SerializeField]
	private tk2dTextMesh _currentCount;

	private InventorySlot _selectedSlot;

	private int _currentItemCount;

	private int _maxItemsCount;

	private Action _onClose;

	public void ShowHowManyWindow(InventorySlot slot, Action onClose = null)
	{
		_onClose = onClose;
		_selectedSlot = slot;
		_maxItemsCount = _selectedSlot.Count;
		base.gameObject.SetActive(true);
		SetToDefault();
	}

	private void SetToDefault()
	{
		_currentItemCount = 0;
		_howManyProgress.Value = 0f;
		_howManyScrollbar.Value = 0f;
		_currentCount.text = "0";
	}

	private void OnDisable()
	{
		_selectedSlot = null;
		if (_onClose != null)
		{
			_onClose();
		}
		base.gameObject.SetActive(false);
	}

	public void HideHowManyWindow()
	{
		_selectedSlot = null;
		SetToDefault();
		if (_onClose != null)
		{
			_onClose();
		}
		base.gameObject.SetActive(false);
	}

	private void OnClickOk()
	{
		if (_currentItemCount > 0)
		{
			string id = _selectedSlot.Item.Id;
			byte ammo = _selectedSlot.Ammo;
			_selectedSlot.Remove(_currentItemCount);
			Transform transform = GameControls.I.Player.transform;
			WorldController.I.Player.PlayerDropItem(transform.position, id, _currentItemCount, ammo);
			GameUIController.I.Inventory.UpdateCurrentPage();
			HideHowManyWindow();
		}
	}

	private void OnClickCancel()
	{
		HideHowManyWindow();
	}

	private void UpdateProgressValue()
	{
		float value = (float)_currentItemCount / (float)_maxItemsCount;
		_howManyProgress.Value = value;
		_howManyScrollbar.Value = value;
	}

	private void OnClickPlus()
	{
		_currentItemCount++;
		_currentCount.text = _currentItemCount.ToString();
		UpdateProgressValue();
	}

	private void OnClickMinus()
	{
		_currentItemCount--;
		_currentCount.text = _currentItemCount.ToString();
		UpdateProgressValue();
	}

	private void OnClickMax()
	{
		_howManyProgress.Value = 1f;
		_howManyScrollbar.Value = 1f;
		_currentItemCount = _maxItemsCount;
		_currentCount.text = _currentItemCount.ToString();
	}

	private void OnSliderChange()
	{
		_howManyProgress.Value = _howManyScrollbar.Value;
		_currentItemCount = Mathf.RoundToInt(_howManyScrollbar.Value * (float)_maxItemsCount);
		_currentCount.text = _currentItemCount.ToString();
	}
}
